using DevilDaggersInfo.Tools.GameMemory;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.OSX;

internal sealed partial class OSXMemoryService(ILogger logger) : INativeMemoryService
{
	private const int _kernSuccess = 0;
	private const int _kernInvalidArgument = 4;
	private const int _kernFailure = 5;

	private const int _vmProtRead = 0x01;
	private const int _vmRegionBasicInfo64 = 9;
	private const int _vmRegionBasicInfoCount64 = 9;

	// Regions are read in chunks rather than whole, because a single region can be gigabytes wide.
	private const int _scanChunkSize = 4 * 1024 * 1024;

	// A full scan reads gigabytes and takes seconds, and it runs from the ~300 Hz render loop, so a scan that came up
	// empty must not be retried on the next frame.
	private const long _rescanCooldownMs = 5000;

	private readonly byte[] _blockBuffer = new byte[MainBlock.Size];

	private bool _loggedWriteFailure;
	private bool _loggedReadFailure;
	private bool _loggedProcessLookupFailure;
	private bool _loggedTaskFailure;
	private bool _loggedScanUnreadable;
	private bool _loggedScanNotFound;

	// The scan buffer is kept between scans rather than reallocated; it is large enough to land on the large object
	// heap. Null until the first scan, so a session that never scans never pays for it.
	private byte[]? _scanBuffer;

	// The address the ddstats block was last found at, alongside the pid it was found in. 0 means nothing is cached.
	private long _blockAddress;
	private int _blockAddressProcessId;

	// Environment.TickCount64 before which no new scan may start, and the status the last scan ended on, which is what
	// callers get while the cooldown holds.
	private long _nextScanTimestamp;
	private BlockAddressStatus _lastScanStatus;

	// task_for_pid is privileged and comparatively expensive, so the port is acquired once and kept for as long as it
	// belongs to the process we are asked about. _taskPort is 0 when nothing is cached.
	private uint _taskPort;
	private int _taskPortProcessId;

	// Our own task port, resolved once. 0 means nothing resolved yet.
	private uint _selfTaskPort;
	private bool _loggedSelfTaskFailure;

	/// <summary>
	/// macOS has no marker offset to fetch. The DevilDaggers.info API serves one for Windows and one for Linux, and
	/// <c>AppOperatingSystem</c> has no third member to ask with, so the block is found by searching for it instead.
	/// </summary>
	public bool RequiresMarkerOffset => false;

	/// <inheritdoc />
	public BlockAddressResult ResolveBlockAddress(Process process, long? ddstatsMarkerOffset)
	{
		// The block does not move while the game runs, so an address found once is reused until it stops reading back
		// as a block - which is what a restart, a different build, or the game freeing it all look like from here.
		if (_blockAddress != 0 && _blockAddressProcessId == process.Id && IsBlockAt(process, _blockAddress))
			return BlockAddressResult.Resolved(_blockAddress);

		if (_blockAddress != 0)
		{
			_blockAddress = 0;
			_blockAddressProcessId = 0;

			// The cached address has stopped reading back as a block. Until a scan says otherwise the honest answer is
			// that there is no block, rather than the outcome of the scan that found the one which has since gone.
			_lastScanStatus = BlockAddressStatus.BlockNotFound;
		}

		if (Environment.TickCount64 < _nextScanTimestamp)
			return new BlockAddressResult(_lastScanStatus, 0);

		_lastScanStatus = ScanForBlock(process, out long scannedAddress);
		_nextScanTimestamp = Environment.TickCount64 + _rescanCooldownMs;

		if (_lastScanStatus != BlockAddressStatus.Resolved)
			return new BlockAddressResult(_lastScanStatus, 0);

		_blockAddress = scannedAddress;
		_blockAddressProcessId = process.Id;
		return BlockAddressResult.Resolved(scannedAddress);
	}

	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		// Mirror ReadMemory in not turning an empty request into a pointer into an empty array.
		if (size <= 0)
			return;

		// A failure here has already been logged by TryGetTaskPort itself.
		if (!TryGetTaskPort(process, out uint task))
			return;

		int kernReturn;
		unsafe
		{
			fixed (byte* localBase = &bytes[offset])
			{
				kernReturn = MachVmWrite(task, (ulong)address, (ulong)localBase, (uint)size);
			}
		}

		if (kernReturn == _kernSuccess)
		{
			_loggedWriteFailure = false;
			return;
		}

		if (kernReturn == _kernInvalidArgument)
		{
			// The task port itself is no longer valid - the game most likely exited. Drop it so the next write
			// re-acquires one instead of failing forever against a dead port.
			_taskPort = 0;
			_taskPortProcessId = 0;
		}

		if (_loggedWriteFailure)
			return;

		_loggedWriteFailure = true;
		logger.Error("Could not write game memory: mach_vm_write returned kern_return_t {KernReturn} ({Description}) writing {Size} bytes at 0x{Address:X8}. Quit ddinfo-tools and start it again under sudo - writing another process's memory requires root on macOS.", kernReturn, DescribeKernReturn(kernReturn), size, address);
	}

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		// Callers can request an empty read (for example when the game reports a replay length of 0), which must not
		// be turned into a pointer into an empty array.
		if (size <= 0)
			return;

		// Callers keep scanning after the block address failed to resolve, and the game leaves the stats and replay
		// pointers null outside a run, so a read of address 0 is routine and is not a fault worth reporting. Why the
		// address is unknown has already been logged by ResolveBlockAddress.
		if (address == 0)
		{
			Array.Clear(bytes, offset, size);
			return;
		}

		if (!TryGetTaskPort(process, out uint task))
		{
			// A read that never happened leaves the buffer holding whatever was in it before, which the callers would
			// happily parse as game state. Clear it so they observe zeroes instead of stale data.
			Array.Clear(bytes, offset, size);
			return;
		}

		int kernReturn = ReadInto(task, (ulong)address, bytes.AsSpan(offset, size), out ulong bytesRead);
		if (kernReturn == _kernSuccess && bytesRead == (ulong)size)
		{
			_loggedReadFailure = false;
			return;
		}

		// A failed or partial read leaves the buffer holding whatever was in it before, which the callers would happily
		// parse as game state. Clear it so they observe zeroes instead of stale data. Refused reads are the common case
		// on macOS, so this matters more here than it does on Linux.
		Array.Clear(bytes, offset, size);

		if (kernReturn == _kernInvalidArgument)
		{
			// The task port itself is no longer valid - the game most likely exited. Drop it so the next read
			// re-acquires one instead of failing forever against a dead port.
			_taskPort = 0;
			_taskPortProcessId = 0;
		}

		if (_loggedReadFailure)
			return;

		_loggedReadFailure = true;
		if (kernReturn != _kernSuccess)
			logger.Error("Could not read game memory: mach_vm_read_overwrite returned kern_return_t {KernReturn} ({Description}) reading {Size} bytes at 0x{Address:X8}. {Region}", kernReturn, DescribeKernReturn(kernReturn), size, address, DescribeRegion(task, (ulong)address));
		else
			logger.Error("Could not read game memory: mach_vm_read_overwrite read {BytesRead} of {Size} requested bytes at 0x{Address:X8}.", bytesRead, size, address);
	}

	public Process? GetDevilDaggersProcess()
	{
		// The macOS process is named "Devil Daggers" - capitals and a space - where the Linux one is "devildaggers",
		// so the name has to be normalised before it can be compared.
		Process? process = Array.Find(Process.GetProcesses(), p => Normalize(p.ProcessName).StartsWith("devildaggers", StringComparison.Ordinal));
		if (process != null)
		{
			_loggedProcessLookupFailure = false;
			return process;
		}

		if (!_loggedProcessLookupFailure)
		{
			_loggedProcessLookupFailure = true;
			logger.Error("Could not locate the Devil Daggers process. Start Devil Daggers from Steam, then try again. If it is running, its process name is not 'Devil Daggers' and this build cannot find it.");
		}

		return null;
	}

	/// <summary>
	/// Lowercases and strips spaces so the macOS process name "Devil Daggers" compares equal to "devildaggers".
	/// </summary>
	private static string Normalize(string processName)
	{
		return processName.Replace(" ", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
	}

	/// <summary>
	/// Walks every readable region of the game's address space looking for the ddstats block.
	/// </summary>
	/// <returns>
	/// <see cref="BlockAddressStatus.Resolved" />, having set <paramref name="blockAddress" />, or the reason the block
	/// could not be found. A scan that could not read a single byte is a permissions problem and is reported separately
	/// from a scan that read the whole address space and found no block in it.
	/// </returns>
	private BlockAddressStatus ScanForBlock(Process process, out long blockAddress)
	{
		blockAddress = 0;

		// TryGetTaskPort logs the sudo advice for us when this fails.
		if (!TryGetTaskPort(process, out uint task))
			return BlockAddressStatus.MemoryUnreadable;

		byte[] buffer = _scanBuffer ??= new byte[_scanChunkSize];
		int markerLength = MainBlock.MarkerBytes.Length;

		ulong regionAddress = 0;
		ulong bytesRead = 0;
		int regionsSeen = 0;
		int regionsRead = 0;

		while (true)
		{
			// Walked off the top of the address space.
			if (!TryGetRegion(task, ref regionAddress, out ulong regionSize, out int protection))
				break;

			regionsSeen++;

			// A zero-width region would leave the walk asking about the same address forever.
			if (regionSize == 0)
				break;

			if ((protection & _vmProtRead) == 0)
			{
				regionAddress += regionSize;
				continue;
			}

			bool regionRead = false;
			ulong offset = 0;
			while (offset < regionSize)
			{
				int chunkSize = (int)Math.Min((ulong)_scanChunkSize, regionSize - offset);
				bool isLastChunk = (ulong)chunkSize == regionSize - offset;

				int kernReturn = ReadInto(task, regionAddress + offset, buffer.AsSpan(0, chunkSize), out ulong chunkBytesRead);
				if (kernReturn == _kernSuccess && chunkBytesRead > 0)
				{
					regionRead = true;
					bytesRead += chunkBytesRead;

					long found = FindBlock(process, buffer.AsSpan(0, (int)chunkBytesRead), (long)(regionAddress + offset));
					if (found != 0)
					{
						_loggedScanUnreadable = false;
						_loggedScanNotFound = false;
						logger.Information("Found the ddstats block at 0x{BlockAddress:X8} in Devil Daggers (process {ProcessId}), after reading {MegabytesRead} MB across {RegionsSeen} memory regions.", found, process.Id, bytesRead / (1024 * 1024), regionsSeen);

						blockAddress = found;
						return BlockAddressStatus.Resolved;
					}
				}

				if (isLastChunk)
					break;

				// Overlap the next chunk by the marker length so a marker straddling the boundary is still found. This
				// branch only runs on a full-size chunk, so it always moves forward.
				offset += (ulong)chunkSize - (ulong)(markerLength - 1);
			}

			if (regionRead)
				regionsRead++;

			regionAddress += regionSize;
		}

		if (bytesRead == 0)
		{
			_loggedScanNotFound = false;
			if (!_loggedScanUnreadable)
			{
				_loggedScanUnreadable = true;
				logger.Error("Could not read the memory of Devil Daggers (process {ProcessId}): a task port was acquired, but all {RegionsSeen} of its memory regions refused to be read. Quit ddinfo-tools and start it again under sudo.", process.Id, regionsSeen);
			}

			return BlockAddressStatus.MemoryUnreadable;
		}

		_loggedScanUnreadable = false;
		if (!_loggedScanNotFound)
		{
			_loggedScanNotFound = true;
			logger.Error("Read {MegabytesRead} MB across {RegionsRead} of {RegionsSeen} memory regions of Devil Daggers (process {ProcessId}), but found no ddstats block in it. Reading the game's memory works, so this is not a permissions problem: the game either has not created the block yet, or is a build whose layout this app does not know. Retrying every {RescanCooldownSeconds} seconds.", bytesRead / (1024 * 1024), regionsRead, regionsSeen, process.Id, _rescanCooldownMs / 1000);
		}

		return BlockAddressStatus.BlockNotFound;
	}

	/// <summary>
	/// Searches one chunk of the game's memory for the ddstats block, rejecting the copies of the marker that are not
	/// one - the game's own string literal, for instance.
	/// </summary>
	/// <returns>The address of the block, or 0 when this chunk holds none.</returns>
	private long FindBlock(Process process, ReadOnlySpan<byte> chunk, long chunkAddress)
	{
		ReadOnlySpan<byte> marker = MainBlock.MarkerBytes;

		int searchFrom = 0;
		while (searchFrom <= chunk.Length - marker.Length)
		{
			int index = chunk[searchFrom..].IndexOf(marker);
			if (index < 0)
				return 0;

			index += searchFrom;

			long candidate = chunkAddress + index;
			if (IsBlockAt(process, candidate))
				return candidate;

			searchFrom = index + 1;
		}

		return 0;
	}

	/// <summary>
	/// Reads the block at <paramref name="address" /> and checks that it is one, both to reject false hits during a
	/// scan and to confirm a cached address still holds the block.
	/// </summary>
	/// <remarks>
	/// This deliberately does not go through <see cref="ReadMemory" />: a candidate that turns out not to be a block,
	/// or a block that has gone away because the game exited, is an expected answer rather than a failure worth
	/// logging.
	/// </remarks>
	private bool IsBlockAt(Process process, long address)
	{
		if (!TryGetTaskPort(process, out uint task))
			return false;

		int kernReturn = ReadInto(task, (ulong)address, _blockBuffer, out ulong bytesRead);
		return kernReturn == _kernSuccess && bytesRead == MainBlock.Size && MainBlock.IsValid(_blockBuffer);
	}

	/// <summary>
	/// Reads our own Mach task port, which <c>task_for_pid</c> needs as its first argument.
	/// </summary>
	/// <remarks>
	/// libsystem_kernel exports <c>mach_task_self</c> as a real function, which <see cref="MachTaskSelf"/> binds, but
	/// that export is a compatibility shim rather than the documented contract: in <c>&lt;mach/mach_init.h&gt;</c> the
	/// call is a macro over an extern variable.
	/// <code>
	/// extern mach_port_t mach_task_self_;
	/// #define mach_task_self() mach_task_self_
	/// </code>
	/// So the function is tried first and the variable is read as a fallback if a future macOS ever drops the export.
	/// Both routes are wrapped, because an unresolvable P/Invoke throws at its call site - which for a memory service
	/// is inside GameMemoryServiceWrapper.Scan() on the ~300 Hz render loop, where an escaping exception would take the
	/// whole app down instead of reporting the problem.
	/// </remarks>
	private bool TryGetSelfTaskPort(out uint selfTask)
	{
		if (_selfTaskPort != 0)
		{
			selfTask = _selfTaskPort;
			return true;
		}

		selfTask = 0;

		string functionFailure = TryReadSelfTaskPortFromFunction(out selfTask);
		if (functionFailure.Length > 0)
		{
			string variableFailure = TryReadSelfTaskPortFromVariable(out selfTask);
			if (variableFailure.Length > 0)
			{
				selfTask = 0;

				if (_loggedSelfTaskFailure)
					return false;

				_loggedSelfTaskFailure = true;
				logger.Error("Could not read game memory: could not look up this process's own Mach task port. mach_task_self() failed because {FunctionFailure}, and reading the mach_task_self_ variable failed because {VariableFailure}. This build cannot read Devil Daggers' memory on this version of macOS.", functionFailure, variableFailure);
				return false;
			}
		}

		_selfTaskPort = selfTask;
		_loggedSelfTaskFailure = false;
		return true;
	}

	/// <summary>
	/// Calls the exported <c>mach_task_self</c> function. Returns an empty string on success, or a description of why
	/// it could not be used.
	/// </summary>
	private static string TryReadSelfTaskPortFromFunction(out uint selfTask)
	{
		selfTask = 0;

		try
		{
			selfTask = MachTaskSelf();
		}
		catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
		{
			return string.Create(CultureInfo.InvariantCulture, $"the symbol could not be resolved ({ex.GetType().Name}: {ex.Message})");
		}

		if (selfTask != 0)
			return string.Empty;

		return "it returned a null port";
	}

	/// <summary>
	/// Reads the <c>mach_task_self_</c> extern variable that <c>mach_task_self()</c> is a macro over. Returns an empty
	/// string on success, or a description of why it could not be used.
	/// </summary>
	private static string TryReadSelfTaskPortFromVariable(out uint selfTask)
	{
		selfTask = 0;

		try
		{
			nint export = NativeLibrary.GetExport(NativeLibrary.Load("libc"), "mach_task_self_");

			// mach_port_t is a 32-bit unsigned integer.
			selfTask = (uint)Marshal.ReadInt32(export);
		}
		catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException or ArgumentNullException)
		{
			return string.Create(CultureInfo.InvariantCulture, $"resolving the symbol threw {ex.GetType().Name}: {ex.Message}");
		}

		if (selfTask != 0)
			return string.Empty;

		return "the symbol holds a null port";
	}

	private bool TryGetTaskPort(Process process, out uint task)
	{
		if (_taskPort != 0 && _taskPortProcessId == process.Id)
		{
			task = _taskPort;
			return true;
		}

		// Anything cached belongs to a different process (the game was restarted); it is useless now.
		_taskPort = 0;
		_taskPortProcessId = 0;

		if (!TryGetSelfTaskPort(out uint selfTask))
		{
			task = 0;
			return false;
		}

		int kernReturn = TaskForPid(selfTask, process.Id, out uint acquiredTask);
		if (kernReturn == _kernSuccess && acquiredTask != 0)
		{
			_taskPort = acquiredTask;
			_taskPortProcessId = process.Id;
			_loggedTaskFailure = false;
			task = acquiredTask;
			return true;
		}

		task = 0;

		if (_loggedTaskFailure)
			return false;

		_loggedTaskFailure = true;
		if (kernReturn == _kernFailure)
			logger.Error("Could not access the memory of Devil Daggers (process {ProcessId}): task_for_pid returned KERN_FAILURE (5), which on macOS means the request was not permitted. Quit ddinfo-tools and start it again under sudo - reading another process's memory requires root on macOS.", process.Id);
		else
			logger.Error("Could not access the memory of Devil Daggers (process {ProcessId}): task_for_pid returned kern_return_t {KernReturn} ({Description}). If ddinfo-tools was not started under sudo, start it again under sudo - reading another process's memory requires root on macOS.", process.Id, kernReturn, DescribeKernReturn(kernReturn));

		return false;
	}

	/// <summary>
	/// Reads as much of the task's memory as <paramref name="destination" /> holds into it. This is the only place
	/// that pins a buffer and calls <c>mach_vm_read_overwrite</c>; what a refusal or a short read means is the
	/// caller's business, because it differs - a scan probing a region expects both, a block read does not.
	/// </summary>
	/// <returns>The <c>kern_return_t</c> the read returned.</returns>
	private static int ReadInto(uint task, ulong address, Span<byte> destination, out ulong bytesRead)
	{
		ulong read = 0;
		int kernReturn;
		unsafe
		{
			fixed (byte* localBase = destination)
			{
				kernReturn = MachVmReadOverwrite(task, address, (ulong)destination.Length, (ulong)localBase, ref read);
			}
		}

		bytesRead = read;
		return kernReturn;
	}

	/// <summary>
	/// Describes the mapping the address falls in, so a refused read can be told apart from a read of memory the game
	/// never mapped in the first place.
	/// </summary>
	private static string DescribeRegion(uint task, ulong address)
	{
		ulong regionAddress = address;
		if (!TryGetRegion(task, ref regionAddress, out _, out int protection))
			return "The address lies above every mapped region in the process.";

		// mach_vm_region reports the first region at or above the requested address, so a higher start address means
		// the requested address is not mapped at all.
		if (regionAddress > address)
			return string.Create(CultureInfo.InvariantCulture, $"The address is not mapped; the nearest mapping starts at 0x{regionAddress:X8}.");

		return (protection & _vmProtRead) == 0
			? string.Create(CultureInfo.InvariantCulture, $"The address lies in an unreadable region at 0x{regionAddress:X8} (protection 0x{protection:X}).")
			: string.Create(CultureInfo.InvariantCulture, $"The address lies in a readable region at 0x{regionAddress:X8} (protection 0x{protection:X}), so the read itself was refused.");
	}

	/// <summary>
	/// Looks up the first region the task has mapped at or above <paramref name="address" />, which
	/// <c>mach_vm_region</c> reports by moving <paramref name="address" /> to the start of the region it found. This
	/// is the only place that stack-allocates the info block and knows which of its fields holds the protection.
	/// </summary>
	/// <returns>
	/// Whether a region was found. False means the address is above everything the process has mapped, which is how
	/// a walk of the whole address space reaches its end.
	/// </returns>
	private static bool TryGetRegion(uint task, ref ulong address, out ulong size, out int protection)
	{
		ulong regionSize = 0;
		int infoCount = _vmRegionBasicInfoCount64;

		int kernReturn;
		unsafe
		{
			int* info = stackalloc int[_vmRegionBasicInfoCount64];
			kernReturn = MachVmRegion(task, ref address, ref regionSize, _vmRegionBasicInfo64, info, ref infoCount, out _);

			// The first field of vm_region_basic_info_64 is the current protection.
			protection = kernReturn == _kernSuccess ? info[0] : 0;
		}

		size = regionSize;
		return kernReturn == _kernSuccess;
	}

	private static string DescribeKernReturn(int kernReturn)
	{
		return kernReturn switch
		{
			1 => "KERN_INVALID_ADDRESS",
			2 => "KERN_PROTECTION_FAILURE",
			_kernInvalidArgument => "KERN_INVALID_ARGUMENT, usually a dead or unknown process",
			_kernFailure => "KERN_FAILURE, almost always 'not permitted'",
			_ => "see mach/kern_return.h",
		};
	}

	// macOS has no process_vm_readv equivalent; reading another process goes through Mach. These signatures were
	// verified against the running game. Mach calls report failure through their kern_return_t result rather than
	// errno, so there is nothing to gain from SetLastError here.
	[LibraryImport("libc", EntryPoint = "mach_task_self")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static partial uint MachTaskSelf();

	[LibraryImport("libc", EntryPoint = "task_for_pid")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static partial int TaskForPid(uint targetTport, int pid, out uint task);

	[LibraryImport("libc", EntryPoint = "mach_vm_read_overwrite")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static partial int MachVmReadOverwrite(uint task, ulong address, ulong size, ulong data, ref ulong outSize);

	[LibraryImport("libc", EntryPoint = "mach_vm_write")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static partial int MachVmWrite(uint task, ulong address, ulong data, uint dataCount);

	[LibraryImport("libc", EntryPoint = "mach_vm_region")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static unsafe partial int MachVmRegion(uint task, ref ulong address, ref ulong size, int flavor, int* info, ref int infoCount, out uint objectName);
}
