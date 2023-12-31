namespace DevilDaggersInfo.Tools.Engine.Parsers.Texture;

public class TgaParseException : Exception
{
	public TgaParseException()
	{
	}

	public TgaParseException(string? message)
		: base(message)
	{
	}

	public TgaParseException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
