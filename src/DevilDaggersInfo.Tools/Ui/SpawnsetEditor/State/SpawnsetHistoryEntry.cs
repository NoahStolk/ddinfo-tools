using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.State;

public record SpawnsetHistoryEntry(SpawnsetBinary Spawnset, byte[] Hash, SpawnsetEditType EditType);
