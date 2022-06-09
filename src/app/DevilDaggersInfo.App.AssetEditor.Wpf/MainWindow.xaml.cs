using DevilDaggersInfo.App.AssetEditor.Wpf.Services;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.NativeInterface;
using DevilDaggersInfo.Razor.Core.AssetEditor.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DevilDaggersInfo.App.AssetEditor.Wpf;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		ServiceCollection serviceCollection = new();
		serviceCollection.AddWpfBlazorWebView();
		serviceCollection.AddSingleton<INativeErrorReporter, NativeErrorReporter>();
		serviceCollection.AddSingleton<INativeFileSystemService, NativeFileSystemService>();
		serviceCollection.AddSingleton<IAssetEditorFileFilterService, AssetEditorFileFilterService>();
		serviceCollection.AddSingleton<BinaryState>();
		Resources.Add("services", serviceCollection.BuildServiceProvider());

		InitializeComponent();

		Title = $"Devil Daggers Asset Editor {VersionUtils.EntryAssemblyVersion}";
		Width = SystemParameters.WorkArea.Width * 0.6;
		Height = SystemParameters.WorkArea.Height * 0.6;
	}
}
