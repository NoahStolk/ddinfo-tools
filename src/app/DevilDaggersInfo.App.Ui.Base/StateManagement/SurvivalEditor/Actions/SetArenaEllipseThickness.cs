namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaEllipseThickness(int Thickness) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaEllipseState = stateReducer.ArenaEllipseState with
		{
			Thickness = Thickness,
		};
	}
}
