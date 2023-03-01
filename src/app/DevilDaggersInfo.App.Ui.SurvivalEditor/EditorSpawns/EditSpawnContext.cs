using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Types.Core.Spawnsets;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorSpawns;

public static class EditSpawnContext
{
	public static List<SpawnUiEntry> GetFrom(SpawnsetBinary spawnsetBinary)
	{
		return GetFrom(spawnsetBinary.Spawns, spawnsetBinary.SpawnVersion, spawnsetBinary.HandLevel, spawnsetBinary.AdditionalGems, spawnsetBinary.TimerStart);
	}

	private static List<SpawnUiEntry> GetFrom(ImmutableArray<Spawn> spawns, int spawnVersion, HandLevel handLevel = HandLevel.Level1, int additionalGems = 0, float timerStart = 0)
	{
		if (spawns.Length == 0)
			return new();

		double totalSeconds = SpawnsetBinary.GetEffectiveTimerStart(spawnVersion, timerStart);
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetBinary.GetEffectivePlayerSettings(spawnVersion, handLevel, additionalGems);
		GemState gemState = new(effectivePlayerSettings.HandLevel, effectivePlayerSettings.GemsOrHoming, 0);

		return Build(ref totalSeconds, ref gemState, spawns);
	}

	private static List<SpawnUiEntry> Build(ref double totalSeconds, ref GemState gemState, ImmutableArray<Spawn> preLoopSpawns)
	{
		int i = 0;

		List<SpawnUiEntry> spawns = new();
		foreach (Spawn spawn in preLoopSpawns)
		{
			totalSeconds += spawn.Delay;
			int noFarmGems = spawn.EnemyType.GetNoFarmGems();
			gemState = gemState.Add(noFarmGems);
			spawns.Add(new(i++, spawn.EnemyType, spawn.Delay, totalSeconds, noFarmGems, gemState));
		}

		return spawns;
	}
}
