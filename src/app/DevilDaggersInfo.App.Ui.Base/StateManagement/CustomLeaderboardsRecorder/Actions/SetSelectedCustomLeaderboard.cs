namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;

public record SetSelectedCustomLeaderboard(int Id, int SpawnsetId, string SpawnsetName) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.LeaderboardListState = stateReducer.LeaderboardListState with
		{
			SelectedCustomLeaderboard = new(Id, SpawnsetId, SpawnsetName),
		};
	}
}
