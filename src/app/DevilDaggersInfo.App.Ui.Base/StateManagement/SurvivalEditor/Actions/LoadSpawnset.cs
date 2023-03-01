using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

/// <summary>
/// Fires when a new spawnset is loaded from disk or elsewhere.
/// Do not use this when editing an existing spawnset in the UI.
/// This sets the spawnset name and clears the history.
/// </summary>
public record LoadSpawnset(string SpawnsetName, SpawnsetBinary SpawnsetBinary) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = new(SpawnsetName, SpawnsetBinary);

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.Reset);

		StateManager.Dispatch(new SetSpawnSelections(new()));
	}
}
