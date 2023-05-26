namespace DevilDaggersInfo.App.Launcher;

public static class Cmd
{
	public static void WriteLine(ConsoleColor foregroundColor, string text)
	{
		Console.ForegroundColor = foregroundColor;
		Console.WriteLine(text);
	}
}
