using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.View;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorSpawns;

public readonly record struct SpawnUiEntry(int Index, EnemyType EnemyType, double Delay, double Seconds, int NoFarmGems, GemState GemState);
