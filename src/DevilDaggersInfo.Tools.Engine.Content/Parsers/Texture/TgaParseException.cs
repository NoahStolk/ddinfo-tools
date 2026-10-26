namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Texture;

// ReSharper disable UnusedMember.Global
public sealed class TgaParseException : Exception
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
