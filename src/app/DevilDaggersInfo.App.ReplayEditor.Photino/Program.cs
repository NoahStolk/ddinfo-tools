using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.NativeInterface;
using DevilDaggersInfo.Core.NativeInterface.Windows;
using DevilDaggersInfo.Razor.ReplayEditor.Services;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		PhotinoBlazorAppBuilder appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
		appBuilder.Services.AddLogging(builder => builder.AddConsole());

		// TODO: Register Linux services on Linux.
		appBuilder.Services.AddSingleton<INativeFileSystemService, NativeFileSystemService>();

		appBuilder.Services.AddScoped<StateFacade>();

		appBuilder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly, typeof(Razor.ReplayEditor.App).Assembly));

		appBuilder.RootComponents.Add<Razor.ReplayEditor.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle($"Devil Daggers Replay Editor {VersionUtils.EntryAssemblyVersion}");

		AppDomain.CurrentDomain.UnhandledException += (sender, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
