using DevilDaggersInfo.Razor.Core.AssetEditor.Services;
using System.Windows;

namespace DevilDaggersInfo.App.AssetEditor.Wpf.Services;

public class ErrorReporter : IErrorReporter
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
