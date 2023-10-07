using System.Diagnostics;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services;

public interface INativeMemoryService
{
	void WriteMemory(Process process, long address, byte[] bytes, int offset, int size);

	void ReadMemory(Process process, long address, byte[] bytes, int offset, int size);

	Process? GetDevilDaggersProcess();
}
