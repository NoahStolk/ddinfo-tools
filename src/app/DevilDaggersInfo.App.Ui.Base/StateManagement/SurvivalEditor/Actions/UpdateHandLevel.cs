using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateHandLevel(HandLevel HandLevel) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				HandLevel = HandLevel,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.HandLevel);
	}
}
