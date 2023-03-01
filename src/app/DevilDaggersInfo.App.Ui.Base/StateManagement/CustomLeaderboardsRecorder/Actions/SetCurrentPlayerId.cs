namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetCurrentPlayerId(int CurrentPlayerId) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.RecordingState = stateReducer.RecordingState with
		{
			CurrentPlayerId = CurrentPlayerId,
		};
	}
}
