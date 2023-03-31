namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;

public class LinuxDialogService : INativeDialogService
{
	public void ReportError(string message, Exception? exception = null)
	{
		ReportError("Error", message, exception);
	}

	public void ReportError(string title, string message, Exception? exception = null)
	{
		if (exception != null)
			message += Environment.NewLine + exception.Message;

		Console.WriteLine($"{title}: {message}");
	}

	public void ReportMessage(string title, string message)
	{
		Console.WriteLine($"{title}{Environment.NewLine}{message}");
	}

	public bool? PromptYesNo(string title, string message)
	{
		Console.WriteLine($"{title}{Environment.NewLine}{message}");
		Console.WriteLine("Y/N");
		ConsoleKeyInfo key = Console.ReadKey();
		return key.Key switch
		{
			ConsoleKey.Y => true,
			ConsoleKey.N => false,
			_ => null,
		};
	}
}
