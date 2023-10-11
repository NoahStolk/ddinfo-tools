using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.State;

public record SpawnsetHistoryEntry(SpawnsetBinary Spawnset, byte[] Hash, SpawnsetEditType EditType);
