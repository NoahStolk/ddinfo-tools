using DevilDaggersInfo.App.AssetEditor.Photino.Services;
using DevilDaggersInfo.App.Core.NativeInterface.Services;
using DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Razor.AssetEditor.Services;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace DevilDaggersInfo.App.AssetEditor.Photino;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		PhotinoBlazorAppBuilder appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
		appBuilder.Services.AddLogging();

		// TODO: Register Linux services on Linux.
		appBuilder.Services.AddSingleton<INativeErrorReporter, WindowsErrorReporter>();
		appBuilder.Services.AddSingleton<INativeFileSystemService, WindowsFileSystemService>();
		appBuilder.Services.AddSingleton<IAssetEditorFileFilterService, AssetEditorFileFilterService>();

		appBuilder.Services.AddScoped<StateFacade>();

		appBuilder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly, typeof(Razor.AssetEditor.App).Assembly));

		appBuilder.RootComponents.Add<Razor.AssetEditor.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle($"Devil Daggers Asset Editor {VersionUtils.EntryAssemblyVersion}");

		AppDomain.CurrentDomain.UnhandledException += (sender, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
