using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetCategory(CustomLeaderboardCategory Category) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			Category = Category,
		};

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			PageIndex = Math.Clamp(stateReducer.LeaderboardListState.PageIndex, 0, stateReducer.LeaderboardListState.GetMaxPageIndex()),
		};
	}
}
