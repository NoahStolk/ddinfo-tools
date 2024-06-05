using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Windows;

public class WindowsWindowingService : INativeWindowingService
{
	public Vector2 GetWindowPosition()
	{
		int window = FindWindow(null, "Devil Daggers");
		GetWindowRect(window, out Rect rect);
		return new Vector2(rect.Left, rect.Top);
	}

	[DllImport("user32.dll")]
	private static extern int FindWindow(string? lpClassName, string? lpWindowName);

	[DllImport("user32.dll")]
	private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
}
