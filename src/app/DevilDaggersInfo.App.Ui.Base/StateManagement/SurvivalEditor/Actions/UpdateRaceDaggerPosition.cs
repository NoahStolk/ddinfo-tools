using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateRaceDaggerPosition(Vector2 NewRaceDaggerPosition) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				RaceDaggerPosition = NewRaceDaggerPosition,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.RaceDagger);
	}
}
