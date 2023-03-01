using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;

public record UpdateArena(float[,] NewArena, SpawnsetEditType SpawnsetEditType) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.SpawnsetState = stateReducer.SpawnsetState with
		{
			Spawnset = stateReducer.SpawnsetState.Spawnset with
			{
				ArenaTiles = new(stateReducer.SpawnsetState.Spawnset.ArenaDimension, NewArena),
			},
		};

		SpawnsetHistoryUtils.Save(stateReducer, SpawnsetEditType);
	}
}
