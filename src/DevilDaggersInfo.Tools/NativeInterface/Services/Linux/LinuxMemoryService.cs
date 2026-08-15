using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Linux;

internal sealed partial class LinuxMemoryService(ILogger logger) : INativeMemoryService
{
	private bool _loggedReadFailure;

	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		// TODO: Implement.
	}

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		// Callers can request an empty read (for example when the game reports a replay length of 0), which must not
		// be turned into a pointer into an empty array.
		if (size <= 0)
			return;

		nint bytesRead;
		unsafe
		{
			fixed (byte* localBase = &bytes[offset])
			{
				IoVec local = new() { Base = localBase, Len = (nuint)size };
				IoVec remote = new() { Base = (byte*)address, Len = (nuint)size };
				bytesRead = ProcessVmReadv(process.Id, &local, 1, &remote, 1, 0);
			}
		}

		if (bytesRead == size)
		{
			_loggedReadFailure = false;
			return;
		}

		// A failed or partial read leaves the buffer holding whatever was in it before, which the callers would happily
		// parse as game state. Clear it so they observe zeroes instead of stale data.
		Array.Clear(bytes, offset, size);

		if (_loggedReadFailure)
			return;

		_loggedReadFailure = true;
		if (bytesRead < 0)
		{
			int errno = Marshal.GetLastPInvokeError();
			logger.Error("Could not read game memory: process_vm_readv failed with errno {Errno} ({Message}). If this is EPERM (1), the kernel is likely blocking the read; see /proc/sys/kernel/yama/ptrace_scope.", errno, Marshal.GetPInvokeErrorMessage(errno));
		}
		else
		{
			logger.Error("Could not read game memory: process_vm_readv read {BytesRead} of {Size} requested bytes.", bytesRead, size);
		}
	}

	public Process? GetDevilDaggersProcess()
	{
		return Array.Find(Process.GetProcesses(), p => p.ProcessName.StartsWith("devildaggers"));
	}

	// Read memory using linux syscall, this seems to have best cross distro support without having to tinker with OS settings
	[LibraryImport("libc", EntryPoint = "process_vm_readv", SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
	private static unsafe partial nint ProcessVmReadv(
		int pid,
		IoVec* localIov,
		nuint liovcnt,
		IoVec* remoteIov,
		nuint riovcnt,
		nuint flags);

	[StructLayout(LayoutKind.Sequential)]
	private unsafe struct IoVec
	{
		public byte* Base;
		public nuint Len;
	}
}
