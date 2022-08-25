using DevilDaggersInfo.App.Core.NativeInterface.Services;
using DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Razor.SurvivalEditor.Services;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino.Blazor;

namespace DevilDaggersInfo.App.SurvivalEditor.Photino;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		PhotinoBlazorAppBuilder appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
		appBuilder.Services.AddLogging(builder => builder.AddConsole());

		// TODO: Register Linux services on Linux.
		appBuilder.Services.AddSingleton<INativeErrorReporter, WindowsErrorReporter>();
		appBuilder.Services.AddSingleton<INativeFileSystemService, WindowsFileSystemService>();

		appBuilder.Services.AddSingleton<NetworkService>();
		appBuilder.Services.AddScoped<StateFacade>();

		appBuilder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly, typeof(Razor.SurvivalEditor.App).Assembly));

		appBuilder.RootComponents.Add<Razor.SurvivalEditor.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle($"Devil Daggers Survival Editor {VersionUtils.EntryAssemblyVersion}");

		AppDomain.CurrentDomain.UnhandledException += (_, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
