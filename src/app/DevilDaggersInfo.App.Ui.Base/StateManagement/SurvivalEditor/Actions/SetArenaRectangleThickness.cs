namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaRectangleThickness(int Thickness) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaRectangleState = stateReducer.ArenaRectangleState with
		{
			Thickness = Thickness,
		};
	}
}
