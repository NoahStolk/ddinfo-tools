using DevilDaggersInfo.Core.NativeInterface;
using System.Windows;

namespace DevilDaggersInfo.App.AssetEditor.Wpf.Services;

public class NativeErrorReporter : INativeErrorReporter
{
	public void ReportError(Exception exception)
	{
		// TODO: Log exception.
		MessageBox.Show(exception.Message, exception.GetType().Name);
	}

	public void ReportError(string message)
	{
		MessageBox.Show(message, "Error");
	}
}
