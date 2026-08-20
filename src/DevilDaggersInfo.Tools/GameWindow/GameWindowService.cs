using DevilDaggersInfo.Tools.NativeInterface.Services;
using System.Numerics;

namespace DevilDaggersInfo.Tools.GameWindow;

internal sealed class GameWindowService(INativeWindowingService windowingService)
{
	public Vector2 GetWindowPosition()
	{
		return windowingService.GetWindowPosition();
	}
}
