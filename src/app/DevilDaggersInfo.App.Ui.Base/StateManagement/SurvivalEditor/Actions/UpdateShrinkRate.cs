using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateShrinkRate(float ShrinkRate) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				ShrinkRate = ShrinkRate,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.ShrinkRate);
	}
}
