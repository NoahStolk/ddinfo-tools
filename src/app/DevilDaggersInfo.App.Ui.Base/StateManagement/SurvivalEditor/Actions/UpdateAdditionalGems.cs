using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateAdditionalGems(int AdditionalGems) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				AdditionalGems = AdditionalGems,
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType.AdditionalGems);
	}
}
