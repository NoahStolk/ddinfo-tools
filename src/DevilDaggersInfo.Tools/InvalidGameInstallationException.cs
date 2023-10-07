namespace DevilDaggersInfo.Tools;

public class InvalidGameInstallationException : Exception
{
	public InvalidGameInstallationException(string? message)
		: base(message)
	{
	}
}
