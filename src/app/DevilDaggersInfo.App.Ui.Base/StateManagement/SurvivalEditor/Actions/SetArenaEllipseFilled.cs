namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaEllipseFilled(bool Filled) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaEllipseState = stateReducer.ArenaEllipseState with
		{
			Filled = Filled,
		};
	}
}
