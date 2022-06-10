using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;

public class NativeMemoryService : INativeMemoryService
{
	public Process? GetDevilDaggersProcess()
		=> Array.Find(Process.GetProcessesByName("dd"), p => p.MainWindowTitle == "Devil Daggers");

	public void ReadMemory(Process process, long address, byte[] bytes, int offset, int size)
		=> ReadProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);

	public void WriteMemory(Process process, long address, byte[] bytes, int offset, int size)
		=> WriteProcessMemory(process.Handle, new(address), bytes, (uint)size, out _);

	[DllImport("kernel32.dll")]
	internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out uint lpNumberOfBytesRead);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out uint lpNumberOfBytesWritten);
}
