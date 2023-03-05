using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetSorting(LeaderboardListSorting Sorting) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		Dictionary<LeaderboardListSorting, bool> sortingDirections = stateReducer.LeaderboardListState.SortingDirections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		sortingDirections[Sorting] = !sortingDirections[Sorting];

		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			Sorting = Sorting,
			SortingDirections = sortingDirections,
		};
	}
}
