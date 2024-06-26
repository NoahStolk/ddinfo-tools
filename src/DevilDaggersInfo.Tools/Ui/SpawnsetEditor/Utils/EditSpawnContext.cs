using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Spawnset.View;
using System.Collections.Immutable;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

public static class EditSpawnContext
{
	private static readonly List<SpawnUiEntry> _spawns = [];

	public static IReadOnlyList<SpawnUiEntry> Spawns => _spawns;

	public static void BuildFrom(SpawnsetBinary spawnsetBinary)
	{
		if (spawnsetBinary.Spawns.Length == 0)
		{
			_spawns.Clear();
			return;
		}

		double totalSeconds = spawnsetBinary.GetEffectiveTimerStart();
		EffectivePlayerSettings effectivePlayerSettings = spawnsetBinary.GetEffectivePlayerSettings();
		GemState gemState = new(effectivePlayerSettings.HandLevel, effectivePlayerSettings.GemsOrHoming, 0);

		Build(ref totalSeconds, ref gemState, spawnsetBinary.Spawns);
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
			_spawns.Add(new SpawnUiEntry(i++, spawn.EnemyType, spawn.Delay, totalSeconds, noFarmGems, gemState));

			if (_spawns.Count >= SpawnsWindow.MaxSpawns)
				return;
		}
	}
}
