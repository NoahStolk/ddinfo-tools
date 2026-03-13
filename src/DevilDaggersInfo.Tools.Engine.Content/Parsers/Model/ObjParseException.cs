namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Model;

public sealed class ObjParseException : Exception
{
	public ObjParseException()
	{
	}

	public ObjParseException(string? message)
		: base(message)
	{
	}

	public ObjParseException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
