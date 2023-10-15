using DevilDaggersInfo.Tools.AppWindows;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using Silk.NET.Windowing;

namespace DevilDaggersInfo.Tools;

public class Application
{
	public Application()
	{
		UserSettings.Load();
		UserCache.Load();

		if (!Version.TryParse(AssemblyUtils.EntryAssemblyVersion, out Version? appVersion))
			throw new InvalidOperationException("The current version number is invalid.");

		AppVersion = appVersion;

		MainAppWindow = new();

		Root.Application = this;
	}

	public Version AppVersion { get; }

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
