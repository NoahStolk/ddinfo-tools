using DevilDaggersInfo.Api.App;
using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

/// <summary>
/// Fires when the custom leaderboard page has loaded.
/// </summary>
public record PageLoaded(Page<GetCustomLeaderboardForOverview>? Page) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		int totalResults = Page?.TotalResults ?? stateReducer.LeaderboardListState.TotalResults;
		int newMaxPageIndex = (int)Math.Ceiling((totalResults + 1) / (float)stateReducer.LeaderboardListState.PageSize) - 1;

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			IsLoading = false,
			Page = Page,
			MaxPageIndex = newMaxPageIndex,
			PageIndex = Math.Clamp(stateReducer.LeaderboardListState.PageIndex, 0, newMaxPageIndex),
			TotalResults = totalResults,
		};
	}
}
