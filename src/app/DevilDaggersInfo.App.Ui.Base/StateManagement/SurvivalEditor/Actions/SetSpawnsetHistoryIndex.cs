namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

/// <summary>
/// Fires when changing history index (undo, redo, clicking an entry).
/// This also reloads the spawnset.
/// </summary>
public record SetSpawnsetHistoryIndex(int Index) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetHistoryState = stateReducer.SpawnsetHistoryState with
		{
			CurrentIndex = Math.Clamp(Index, 0, stateReducer.SpawnsetHistoryState.History.Count - 1),
		};

		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetHistoryState.History[stateReducer.SpawnsetHistoryState.CurrentIndex].Spawnset.DeepCopy(),
		};

		StateManager.Dispatch(new SetSpawnSelections(new()));
	}
}
