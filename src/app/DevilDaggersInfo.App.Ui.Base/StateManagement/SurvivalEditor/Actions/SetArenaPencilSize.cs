namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaPencilSize(float Size) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaPencilState = new(Size);
	}
}
