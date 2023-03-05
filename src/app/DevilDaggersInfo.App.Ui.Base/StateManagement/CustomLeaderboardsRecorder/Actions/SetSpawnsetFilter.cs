namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetSpawnsetFilter(string SpawnsetFilter) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			SpawnsetFilter = SpawnsetFilter,
		};

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			PageIndex = Math.Clamp(stateReducer.LeaderboardListState.PageIndex, 0, stateReducer.LeaderboardListState.GetMaxPageIndex()),
		};
	}
}
