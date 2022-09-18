using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Tools.States;

public record SpawnsetHistory(SpawnsetBinary Spawnset, byte[] Hash, string Change);
