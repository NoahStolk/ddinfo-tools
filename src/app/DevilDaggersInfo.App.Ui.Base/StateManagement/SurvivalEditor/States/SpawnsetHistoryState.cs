using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

// Note; the history should never be an empty list.
public record SpawnsetHistoryState(IReadOnlyList<SpawnsetHistoryEntry> History, int CurrentIndex)
{
	public static SpawnsetHistoryState GetDefault()
	{
		SpawnsetBinary defaultSpawnset = SpawnsetBinary.CreateDefault();
		byte[] hash = MD5.HashData(defaultSpawnset.ToBytes());
		return new(new List<SpawnsetHistoryEntry> { new(defaultSpawnset, hash, SpawnsetEditType.Reset) }, 0);
	}
}
