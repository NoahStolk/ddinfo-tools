using System.Windows;

namespace DevilDaggersInfo.App.CustomLeaderboard.Wpf;

public partial class App : Application
{
	private void Application_Startup(object sender, StartupEventArgs e)
	{
		AppDomain.CurrentDomain.UnhandledException += (sender, error) => MessageBox.Show(error.ExceptionObject.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
