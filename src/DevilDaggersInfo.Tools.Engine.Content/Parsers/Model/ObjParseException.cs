namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Model;

// ReSharper disable UnusedMember.Global
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
