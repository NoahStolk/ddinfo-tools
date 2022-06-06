using DevilDaggersInfo.App.CustomLeaderboard.Wpf.Services;
using DevilDaggersInfo.Core.NativeInterface;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;

namespace DevilDaggersInfo.App.CustomLeaderboard.Wpf;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		ServiceCollection serviceCollection = new();
		serviceCollection.AddWpfBlazorWebView();
		serviceCollection.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		Resources.Add("services", serviceCollection.BuildServiceProvider());

		InitializeComponent();

		string? wpfVersion = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		Title = $"Devil Daggers Custom Leaderboards {wpfVersion}";
		Width = SystemParameters.WorkArea.Width * 0.6;
		Height = SystemParameters.WorkArea.Height * 0.6;
	}
}
