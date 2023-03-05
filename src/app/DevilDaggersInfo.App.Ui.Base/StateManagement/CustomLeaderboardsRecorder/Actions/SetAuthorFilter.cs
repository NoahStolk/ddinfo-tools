namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetAuthorFilter(string AuthorFilter) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			AuthorFilter = AuthorFilter,
		};

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			PageIndex = Math.Clamp(stateReducer.LeaderboardListState.PageIndex, 0, stateReducer.LeaderboardListState.GetMaxPageIndex()),
		};
	}
}
