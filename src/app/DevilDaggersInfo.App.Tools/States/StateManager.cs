using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Tools.States;

public static class StateManager
{
	public static SpawnsetState SpawnsetState { get; private set; } = SpawnsetState.GetDefault();

	public static ArenaEditorState ArenaEditorState { get; private set; } = ArenaEditorState.GetDefault();

	public static void SetSpawnset(string spawnsetName, SpawnsetBinary spawnsetBinary)
	{
		SpawnsetState = new(spawnsetName, spawnsetBinary);

		Base.Game.MainLayout.SpawnsWrapper.SetSpawnset();
		SpawnsetHistoryManager.Reset();
	}

	public static void SetSpawnset(SpawnsetBinary spawnsetBinary)
	{
		SpawnsetState = SpawnsetState with { Spawnset = spawnsetBinary };

		Base.Game.MainLayout.SpawnsWrapper.SetSpawnset();
	}

	public static void SetArenaTool(ArenaTool arenaTool)
	{
		ArenaEditorState = ArenaEditorState with { ArenaTool = arenaTool };
	}

	public static void SetArenaSelectedHeight(float selectedHeight)
	{
		ArenaEditorState = ArenaEditorState with { SelectedHeight = selectedHeight };
	}
}
