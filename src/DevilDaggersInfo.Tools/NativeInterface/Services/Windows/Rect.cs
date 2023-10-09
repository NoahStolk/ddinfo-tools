using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.NativeInterface.Services.Windows;

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}
