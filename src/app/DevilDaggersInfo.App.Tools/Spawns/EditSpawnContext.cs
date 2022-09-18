using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Types.Core.Spawnsets;

namespace DevilDaggersInfo.App.Tools.Spawns;

public static class EditSpawnContext
{
	public static List<EditableSpawn> GetFrom(SpawnsetBinary spawnsetBinary)
	{
		return GetFrom(spawnsetBinary.Spawns, spawnsetBinary.HandLevel, spawnsetBinary.AdditionalGems, spawnsetBinary.TimerStart);
	}

	public static List<EditableSpawn> GetFrom(Spawn[] spawns, HandLevel handLevel = HandLevel.Level1, int additionalGems = 0, float timerStart = 0)
	{
		if (spawns.Length == 0)
			return new();

		double totalSeconds = timerStart;
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetBinary.GetEffectivePlayerSettings(handLevel, additionalGems);
		GemState gemState = new(effectivePlayerSettings.HandLevel, effectivePlayerSettings.GemsOrHoming, 0);

		return BuildPreLoop(ref totalSeconds, ref gemState, spawns);
	}

	private static List<EditableSpawn> BuildPreLoop(ref double totalSeconds, ref GemState gemState, Spawn[] preLoopSpawns)
	{
		List<EditableSpawn> spawns = new();
		foreach (Spawn spawn in preLoopSpawns)
		{
			totalSeconds += spawn.Delay;
			int noFarmGems = spawn.EnemyType.GetNoFarmGems();
			gemState = gemState.Add(noFarmGems);
			spawns.Add(new(spawn.EnemyType, spawn.Delay, totalSeconds, noFarmGems, gemState));
		}

		return spawns;
	}
}
