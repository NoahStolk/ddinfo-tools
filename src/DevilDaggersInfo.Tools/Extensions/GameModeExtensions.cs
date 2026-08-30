using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class GameModeExtensions
{
	public static string ToDisplayString(this GameMode gameMode)
	{
		return gameMode switch
		{
			GameMode.Survival => "Survival",
			GameMode.TimeAttack => "Time Attack",
			GameMode.Race => "Race",
			_ => $"Invalid ({(int)gameMode})",
		};
	}
}
