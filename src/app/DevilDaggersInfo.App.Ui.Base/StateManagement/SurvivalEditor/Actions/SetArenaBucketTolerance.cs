namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record SetArenaBucketTolerance(float Tolerance) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ArenaBucketState = stateReducer.ArenaBucketState with
		{
			Tolerance = Tolerance,
		};
	}
}
