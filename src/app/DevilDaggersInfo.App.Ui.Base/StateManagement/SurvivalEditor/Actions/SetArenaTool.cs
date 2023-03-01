using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaTool(ArenaTool ArenaTool) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaEditorState = stateReducer.ArenaEditorState with
		{
			ArenaTool = ArenaTool,
		};
	}
}
