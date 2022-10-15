using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Core.NativeInterface.Services;
using DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;
using DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Razor.CustomLeaderboard.Services;
using Fluxor;
using Microsoft.Extensions.Configuration;
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

		IConfiguration configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

		appBuilder.Services.AddSingleton<IClientConfiguration, ClientConfiguration>();
		appBuilder.Services.AddSingleton<IEncryptionService, EncryptionService>();

		// TODO: Register Linux services on Linux.
		appBuilder.Services.AddSingleton<INativeErrorReporter, WindowsErrorReporter>();
		appBuilder.Services.AddSingleton<INativeMemoryService, WindowsMemoryService>();
		appBuilder.Services.AddSingleton<NetworkService>();
		appBuilder.Services.AddSingleton<GameMemoryService>();
		appBuilder.Services.AddSingleton<UploadService>();
		appBuilder.Services.AddSingleton(configuration);
		appBuilder.Services.AddScoped<StateFacade>();

		appBuilder.Services.AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly, typeof(Razor.CustomLeaderboard.App).Assembly));

		appBuilder.RootComponents.Add<Razor.CustomLeaderboard.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle($"Devil Daggers Custom Leaderboards {VersionUtils.EntryAssemblyVersion}");

		AppDomain.CurrentDomain.UnhandledException += (_, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
