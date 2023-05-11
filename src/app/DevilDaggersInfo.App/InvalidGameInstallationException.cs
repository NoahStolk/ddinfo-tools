namespace DevilDaggersInfo.App;

public class InvalidGameInstallationException : Exception
{
	public InvalidGameInstallationException(string? message)
		: base(message)
	{
	}
}
