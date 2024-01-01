namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Sound;

public class WaveParseException : Exception
{
	public WaveParseException()
	{
	}

	public WaveParseException(string? message)
		: base(message)
	{
	}

	public WaveParseException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
