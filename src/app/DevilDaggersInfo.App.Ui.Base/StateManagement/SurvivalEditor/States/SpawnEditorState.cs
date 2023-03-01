namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record SpawnEditorState(IReadOnlyList<int> SelectedIndices)
{
	public static SpawnEditorState GetDefault()
	{
		return new(new List<int>());
	}
}
