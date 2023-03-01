namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;

public class LinuxDialogService : INativeDialogService
{
	public void ReportError(string message, Exception? exception = null)
	{
		throw new NotImplementedException();
	}

	public void ReportError(string title, string message, Exception? exception = null)
	{
		throw new NotImplementedException();
	}

	public void ReportMessage(string title, string message)
	{
		throw new NotImplementedException();
	}

	public bool? PromptYesNo(string title, string message)
	{
		throw new NotImplementedException();
	}
}
