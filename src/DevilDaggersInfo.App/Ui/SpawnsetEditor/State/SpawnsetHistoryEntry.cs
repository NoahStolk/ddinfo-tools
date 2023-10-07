using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.State;

public record SpawnsetHistoryEntry(SpawnsetBinary Spawnset, byte[] Hash, SpawnsetEditType EditType);
