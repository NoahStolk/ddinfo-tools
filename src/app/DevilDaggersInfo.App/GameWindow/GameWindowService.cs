using DevilDaggersInfo.App.Core.NativeInterface.Services;
using System.Numerics;

namespace DevilDaggersInfo.App.GameWindow;

public class GameWindowService
{
	private readonly INativeWindowingService _windowingService;

	public GameWindowService(INativeWindowingService windowingService)
	{
		_windowingService = windowingService;
	}

	public Vector2 GetWindowPosition()
	{
		return _windowingService.GetWindowPosition();
	}
}
