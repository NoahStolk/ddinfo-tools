using DevilDaggersInfo.App.AppWindows;
using DevilDaggersInfo.App.User.Cache;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.Utils;

namespace DevilDaggersInfo.App;

public class Application
{
	private readonly MainAppWindow _mainAppWindow;

	public Application()
	{
		UserSettings.Load();
		UserCache.Load();

		if (!Version.TryParse(AssemblyUtils.EntryAssemblyVersion, out Version? appVersion))
			throw new InvalidOperationException("The current version number is invalid.");

		AppVersion = appVersion;

		_mainAppWindow = new();

		Root.Application = this;
	}

	public Version AppVersion { get; }

	public PerSecondCounter RenderCounter { get; } = new();
	public float LastRenderDelta { get; set; }

	public void Run()
	{
		_mainAppWindow.WindowInstance.Initialize();

		_mainAppWindow.WindowInstance.Run(() =>
		{
			_mainAppWindow.WindowInstance.DoEvents();

			if (!_mainAppWindow.WindowInstance.IsClosing)
				_mainAppWindow.WindowInstance.DoUpdate();

			if (!_mainAppWindow.WindowInstance.IsClosing)
				_mainAppWindow.WindowInstance.DoRender();
		});
	}

	public void Destroy()
	{
		_mainAppWindow.WindowInstance.Dispose();
	}
}
