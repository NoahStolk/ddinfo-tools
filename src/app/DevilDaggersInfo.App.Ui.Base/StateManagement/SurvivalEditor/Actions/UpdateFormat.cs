using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateFormat(int WorldVersion, int SpawnVersion) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				WorldVersion = WorldVersion,
				SpawnVersion = SpawnVersion,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.Format);
	}
}
