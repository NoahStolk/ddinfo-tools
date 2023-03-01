using System.Diagnostics;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;

// TODO: Implement.
public class LinuxMemoryService : INativeMemoryService
{
	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
	}

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
	}

	public Process? GetDevilDaggersProcess()
	{
		return null;
	}
}
