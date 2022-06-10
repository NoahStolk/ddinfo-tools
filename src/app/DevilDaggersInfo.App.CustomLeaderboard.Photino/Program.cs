using DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using DevilDaggersInfo.Core.NativeInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		PhotinoBlazorAppBuilder appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
		appBuilder.Services.AddLogging();

		IConfiguration configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

		appBuilder.Services.AddSingleton<IClientConfiguration, ClientConfiguration>();
		appBuilder.Services.AddSingleton<IEncryptionService, EncryptionService>();
		appBuilder.Services.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		appBuilder.Services.AddSingleton<INativeMemoryService, NativeMemoryService>();
		appBuilder.Services.AddSingleton<NetworkService>();
		appBuilder.Services.AddSingleton<ReaderService>();
		appBuilder.Services.AddSingleton<UploadService>();
		appBuilder.Services.AddSingleton(configuration);

		// register root component
		appBuilder.RootComponents.Add<Razor.Core.CustomLeaderboard.App>("app");

		PhotinoBlazorApp app = appBuilder.Build();

		app.MainWindow
			.SetIconFile("Icon.ico")
			.SetTitle("Devil Daggers Custom Leaderboards");

		AppDomain.CurrentDomain.UnhandledException += (sender, error) => app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());

		app.Run();
	}
}
