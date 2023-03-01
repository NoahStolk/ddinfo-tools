using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor;

public static class SpawnsetHistoryUtils
{
	public static void Save(StateReducer stateReducer, SpawnsetEditType spawnsetEditType)
	{
		if (spawnsetEditType == SpawnsetEditType.Reset)
		{
			SpawnsetBinary copy = stateReducer.SpawnsetState.Spawnset.DeepCopy();
			byte[] hash = MD5.HashData(copy.ToBytes());
			StateManager.Dispatch(new ClearHistory(new(copy, hash, spawnsetEditType)));
		}
		else
		{
			SpawnsetBinary copy = stateReducer.SpawnsetState.Spawnset.DeepCopy();
			byte[] originalHash = stateReducer.SpawnsetHistoryState.History[stateReducer.SpawnsetHistoryState.CurrentIndex].Hash;
			byte[] hash = MD5.HashData(copy.ToBytes());

			if (ArrayUtils.AreEqual(originalHash, hash))
				return;

			StateManager.Dispatch(new AddHistory(new(copy, hash, spawnsetEditType)));
		}
	}
}
