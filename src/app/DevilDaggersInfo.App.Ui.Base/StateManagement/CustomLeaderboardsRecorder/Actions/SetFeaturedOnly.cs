namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetFeaturedOnly(bool FeaturedOnly) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			FeaturedOnly = FeaturedOnly,
		};

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			PageIndex = Math.Clamp(stateReducer.LeaderboardListState.PageIndex, 0, stateReducer.LeaderboardListState.GetMaxPageIndex()),
		};
	}
}
