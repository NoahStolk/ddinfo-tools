using DevilDaggersInfo.App.AssetEditor.Photino.Services;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.NativeInterface;
using DevilDaggersInfo.Razor.AssetEditor.Services;
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

		appBuilder.Services.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		appBuilder.Services.AddSingleton<INativeFileSystemService, NativeFileSystemService>();
		appBuilder.Services.AddSingleton<IAssetEditorFileFilterService, AssetEditorFileFilterService>();
		appBuilder.Services.AddSingleton<BinaryState>();

		appBuilder.RootComponents.Add<Razor.AssetEditor.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle($"Devil Daggers Asset Editor {VersionUtils.EntryAssemblyVersion}");

		AppDomain.CurrentDomain.UnhandledException += (sender, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
