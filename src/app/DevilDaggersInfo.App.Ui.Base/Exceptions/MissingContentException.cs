namespace DevilDaggersInfo.App.Ui.Base.Exceptions;

public class MissingContentException : Exception
{
	public MissingContentException(string? message)
		: base(message)
	{
	}
}
