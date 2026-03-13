namespace DevilDaggersInfo.Tools;

internal class InvalidGameInstallationException : Exception
{
	public InvalidGameInstallationException(string? message)
		: base(message)
	{
	}
}
