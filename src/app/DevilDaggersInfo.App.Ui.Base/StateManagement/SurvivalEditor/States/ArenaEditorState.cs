using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

public record ArenaEditorState(float SelectedHeight, ArenaTool ArenaTool)
{
	public static ArenaEditorState GetDefault()
	{
		return new(0, ArenaTool.Pencil);
	}
}
