namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaLineThickness(float Thickness) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaLineState = new(Thickness);
	}
}
