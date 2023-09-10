using System.Runtime.InteropServices;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}
