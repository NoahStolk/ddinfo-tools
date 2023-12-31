using DevilDaggersInfo.Tools.AppWindows;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using Silk.NET.Windowing;

namespace DevilDaggersInfo.Tools;

public class Application
{
	public Application()
	{
		UserSettings.Load();
		UserCache.Load();

		MainAppWindow = new();

		Root.Application = this;
	}

	public PerSecondCounter RenderCounter { get; } = new();
	public float LastRenderDelta { get; set; }

	public MainAppWindow MainAppWindow { get; }

	public void Run()
	{
		MainAppWindow.WindowInstance.Run();
	}

	public void Destroy()
	{
		MainAppWindow.WindowInstance.Dispose();
	}
}
