using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.View;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

public readonly record struct SpawnUiEntry(int Index, EnemyType EnemyType, double Delay, double Seconds, int NoFarmGems, GemState GemState);
