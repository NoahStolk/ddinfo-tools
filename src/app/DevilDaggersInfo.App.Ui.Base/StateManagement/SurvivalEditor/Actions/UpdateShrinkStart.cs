using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateShrinkStart(float ShrinkStart) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				ShrinkStart = ShrinkStart,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.ShrinkStart);
	}
}
