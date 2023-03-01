namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaRectangleFilled(bool Filled) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaRectangleState = stateReducer.ArenaRectangleState with
		{
			Filled = Filled,
		};
	}
}
