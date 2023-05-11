using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.State;

public static class SpawnsetState
{
	public static SpawnsetBinary Spawnset { get; set; } = SpawnsetBinary.CreateDefault();

	public static string SpawnsetName { get; set; } = "(untitled)";
}
