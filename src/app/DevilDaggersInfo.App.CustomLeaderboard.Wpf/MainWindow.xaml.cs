using DevilDaggersInfo.App.CustomLeaderboard.Wpf.Services;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;
using DevilDaggersInfo.Core.NativeInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevilDaggersInfo.App.CustomLeaderboard.Wpf;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		IConfiguration configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

		ServiceCollection serviceCollection = new();
		serviceCollection.AddWpfBlazorWebView();
		serviceCollection.AddSingleton<IClientConfiguration, ClientConfiguration>();
		serviceCollection.AddSingleton<IEncryptionService, EncryptionService>();
		serviceCollection.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		serviceCollection.AddSingleton<INativeMemoryService, NativeMemoryService>();
		serviceCollection.AddSingleton<NetworkService>();
		serviceCollection.AddSingleton<ReaderService>();
		serviceCollection.AddSingleton<UploadService>();
		serviceCollection.AddSingleton(configuration);

		Resources.Add("services", serviceCollection.BuildServiceProvider());

		InitializeComponent();

		Title = $"Devil Daggers Custom Leaderboards {VersionUtils.EntryAssemblyVersion}";
		Width = SystemParameters.WorkArea.Width * 0.6;
		Height = SystemParameters.WorkArea.Height * 0.6;
	}
}
