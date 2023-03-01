namespace DevilDaggersInfo.App.Core.NativeInterface.Services;

public interface INativeDialogService
{
	void ReportError(string message, Exception? exception = null);

	void ReportError(string title, string message, Exception? exception = null);

	void ReportMessage(string title, string message);

	bool? PromptYesNo(string title, string message);
}
