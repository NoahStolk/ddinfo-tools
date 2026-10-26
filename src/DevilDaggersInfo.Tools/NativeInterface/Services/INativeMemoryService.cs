using DevilDaggersInfo.Web.ApiSpec.Tools;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.NativeInterface.Services;

internal interface INativeMemoryService
{
	void WriteMemory(Process process, long address, byte[] bytes, int offset, int size);

	void ReadMemory(Process process, long address, byte[] bytes, int offset, int size);

	Process? GetDevilDaggersProcess();

	/// <summary>
	/// Returns the address the game executable is loaded at, or <see langword="null"/> if it cannot be determined.
	/// </summary>
	long? GetGameBaseAddress(Process process);

	/// <summary>
	/// Returns which build of the game the process is running. This is not necessarily the OS the app is running on, since the Windows build can also run on Linux through Proton.
	/// </summary>
	AppOperatingSystem GetGameOperatingSystem(Process process);
}
