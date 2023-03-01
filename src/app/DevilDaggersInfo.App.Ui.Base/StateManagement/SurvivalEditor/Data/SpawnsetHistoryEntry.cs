using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;

public record SpawnsetHistoryEntry(SpawnsetBinary Spawnset, byte[] Hash, SpawnsetEditType EditType);
