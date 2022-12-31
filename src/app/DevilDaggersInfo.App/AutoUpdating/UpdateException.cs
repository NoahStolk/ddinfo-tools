namespace DevilDaggersInfo.App.AutoUpdating;

public class UpdateException : Exception
{
	public UpdateException(string message)
		: base(message)
	{
	}
}
