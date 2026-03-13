namespace DevilDaggersInfo.Tools;

internal sealed class InvalidGameInstallationException : Exception
{
	public InvalidGameInstallationException(string? message)
		: base(message)
	{
	}
}
