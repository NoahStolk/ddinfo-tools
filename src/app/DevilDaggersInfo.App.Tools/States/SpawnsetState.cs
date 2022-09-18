using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Tools.States;

public record SpawnsetState(string SpawnsetName, SpawnsetBinary Spawnset)
{
	public static SpawnsetState GetDefault()
	{
		return new("(untitled)", SpawnsetBinary.CreateDefault());
	}
}
