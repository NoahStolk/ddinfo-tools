using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record ClearHistory(SpawnsetHistoryEntry InitialHistoryEntry) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetHistoryState = new(new List<SpawnsetHistoryEntry> { InitialHistoryEntry }, 0);
	}
}
