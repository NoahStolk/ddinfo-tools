using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Windows;

public class WindowsMemoryService : INativeMemoryService
{
	public Process? GetDevilDaggersProcess()
	{
		Process[] ddProcesses = Process.GetProcessesByName("dd");
		return Array.Find(ddProcesses, p => p.MainWindowTitle == "Devil Daggers");
	}

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);
	}

	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
	{
		WriteProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);
	}

	// TODO: Use source-generated P/Invoke.
	[DllImport("kernel32.dll")]
	private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out uint lpNumberOfBytesRead);

	// TODO: Use source-generated P/Invoke.
	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out uint lpNumberOfBytesWritten);
}
