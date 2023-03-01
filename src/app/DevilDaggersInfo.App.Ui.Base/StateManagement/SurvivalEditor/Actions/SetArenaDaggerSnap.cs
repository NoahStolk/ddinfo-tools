namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaDaggerSnap(Vector2 Snap) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaDaggerState = new(Snap);
	}
}
