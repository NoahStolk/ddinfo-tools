using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Linux;

internal sealed partial class LinuxMemoryService : INativeMemoryService
{
	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		// TODO: Implement.
	}

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		unsafe
		{
			fixed (byte* localBase = &bytes[offset])
			{
				IoVec local = new() { Base = localBase, Len = (nuint)size };
				IoVec remote = new() { Base = (byte*)address, Len = (nuint)size };
				ProcessVmReadv(process.Id, &local, 1, &remote, 1, 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private unsafe struct IoVec
	{
		public byte* Base;
		public nuint Len;
	}

	// Read memory using linux syscall, this seems to have best cross distro support without having to tinker with OS settings
	[LibraryImport("libc", EntryPoint = "process_vm_readv", SetLastError = true)]
	private static unsafe partial long ProcessVmReadv(
		int pid,
		IoVec* localIov,
		ulong liovcnt,
		IoVec* remoteIov,
		ulong riovcnt,
		ulong flags);

	public Process? GetDevilDaggersProcess()
	{
		return Array.Find(Process.GetProcesses(), p => p.ProcessName.StartsWith("devildaggers"));
	}
}
