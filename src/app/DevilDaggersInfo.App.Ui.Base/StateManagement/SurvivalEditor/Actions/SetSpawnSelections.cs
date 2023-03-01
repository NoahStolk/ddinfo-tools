namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetSpawnSelections(List<int> SelectedIndices) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnEditorState = new(SelectedIndices);
	}
}
