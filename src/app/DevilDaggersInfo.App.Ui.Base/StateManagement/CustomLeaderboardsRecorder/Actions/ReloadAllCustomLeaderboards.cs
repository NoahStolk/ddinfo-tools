namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record ReloadAllCustomLeaderboards : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			IsLoading = true,
		};
	}
}
