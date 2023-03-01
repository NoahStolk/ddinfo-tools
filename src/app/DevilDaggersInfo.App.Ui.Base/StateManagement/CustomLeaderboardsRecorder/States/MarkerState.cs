namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.States;

public record MarkerState(long? Marker)
{
	public static MarkerState GetDefault()
	{
		return new((long?)null);
	}
}
