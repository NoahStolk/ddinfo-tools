using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;

[SupportedOSPlatform("windows5.0")]
public class WindowsDialogService : INativeDialogService
{
	public void ReportError(string message, Exception? exception = null)
	{
		ReportError("Error", message, exception);
	}

	public void ReportError(string title, string message, Exception? exception = null)
	{
		if (exception != null)
			message += Environment.NewLine + exception.Message;

		PInvoke.MessageBox(HWND.Null, message, title, MESSAGEBOX_STYLE.MB_OK);
	}

	public void ReportMessage(string title, string message)
	{
		PInvoke.MessageBox(HWND.Null, message, title, MESSAGEBOX_STYLE.MB_OK);
	}

	public bool? PromptYesNo(string title, string message)
	{
		return PInvoke.MessageBox(HWND.Null, message, title, MESSAGEBOX_STYLE.MB_YESNO) switch
		{
			MESSAGEBOX_RESULT.IDYES => true,
			MESSAGEBOX_RESULT.IDNO => false,
			_ => null,
		};
	}
}
