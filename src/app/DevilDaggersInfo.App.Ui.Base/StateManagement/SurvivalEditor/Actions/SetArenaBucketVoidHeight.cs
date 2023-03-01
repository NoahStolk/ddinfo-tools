namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaBucketVoidHeight(float VoidHeight) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaBucketState = stateReducer.ArenaBucketState with
		{
			VoidHeight = VoidHeight,
		};
	}
}
