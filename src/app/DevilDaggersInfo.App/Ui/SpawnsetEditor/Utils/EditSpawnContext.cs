using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Spawnset.View;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;

public static class EditSpawnContext
{
	private static readonly List<SpawnUiEntry> _spawns = new();

	public static IEnumerable<SpawnUiEntry> Spawns => _spawns;

	public static void BuildFrom(SpawnsetBinary spawnsetBinary)
	{
		BuildFrom(spawnsetBinary.Spawns, spawnsetBinary.SpawnVersion, spawnsetBinary.HandLevel, spawnsetBinary.AdditionalGems, spawnsetBinary.TimerStart);
	}

	private static void BuildFrom(ImmutableArray<Spawn> spawns, int spawnVersion, HandLevel handLevel = HandLevel.Level1, int additionalGems = 0, float timerStart = 0)
	{
		if (spawns.Length == 0)
		{
			_spawns.Clear();
			return;
		}

		double totalSeconds = SpawnsetUtils.GetEffectiveTimerStart(spawnVersion, timerStart);
		EffectivePlayerSettings effectivePlayerSettings = SpawnsetUtils.GetEffectivePlayerSettings(spawnVersion, handLevel, additionalGems);
		GemState gemState = new(effectivePlayerSettings.HandLevel, effectivePlayerSettings.GemsOrHoming, 0);

		Build(ref totalSeconds, ref gemState, spawns);
	}

	private static void Build(ref double totalSeconds, ref GemState gemState, ImmutableArray<Spawn> preLoopSpawns)
	{
		int i = 0;

		_spawns.Clear();
		foreach (Spawn spawn in preLoopSpawns)
		{
			totalSeconds += spawn.Delay;
			int noFarmGems = spawn.EnemyType.GetNoFarmGems();
			gemState = gemState.Add(noFarmGems);
			_spawns.Add(new(i++, spawn.EnemyType, spawn.Delay, totalSeconds, noFarmGems, gemState));

			if (_spawns.Count >= SpawnsChild.MaxSpawns)
				return;
		}
	}
}
