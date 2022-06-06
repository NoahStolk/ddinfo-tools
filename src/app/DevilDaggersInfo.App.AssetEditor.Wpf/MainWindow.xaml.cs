using DevilDaggersInfo.App.AssetEditor.Wpf.Services;
using DevilDaggersInfo.Razor.Core.AssetEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;

namespace DevilDaggersInfo.App.AssetEditor.Wpf;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		ServiceCollection serviceCollection = new();
		serviceCollection.AddWpfBlazorWebView();
		serviceCollection.AddSingleton<IErrorReporter, ErrorReporter>();
		serviceCollection.AddSingleton<IFileSystemService, FileSystemService>();
		serviceCollection.AddSingleton<BinaryState>();
		Resources.Add("services", serviceCollection.BuildServiceProvider());

		InitializeComponent();

		string? wpfVersion = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		Title = $"Devil Daggers Asset Editor {wpfVersion}";
		Width = SystemParameters.WorkArea.Width * 0.6;
		Height = SystemParameters.WorkArea.Height * 0.6;
	}
}
