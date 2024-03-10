using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Windows;

[SupportedOSPlatform("windows5.1.2600")]
public class WindowsMemoryService : INativeMemoryService
{
	public Process? GetDevilDaggersProcess()
	{
		Process[] ddProcesses = Process.GetProcessesByName("dd");
		return Array.Find(ddProcesses, p => p.MainWindowTitle == "Devil Daggers");
	}

	public unsafe void ReadMemory(Process process, long address, Span<byte> bytes, int offset, int size)
	{
		fixed (byte* pBuffer = bytes)
			PInvoke.ReadProcessMemory(process.SafeHandle, (void*)address, pBuffer, (nuint)size, (nuint*)0);
	}

	public unsafe void WriteMemory(Process process, long address, Span<byte> bytes, int offset, int size)
	{
		fixed (byte* pBuffer = bytes)
			PInvoke.WriteProcessMemory(process.SafeHandle, (void*)address, pBuffer, (nuint)size, (nuint*)0);
	}
}
