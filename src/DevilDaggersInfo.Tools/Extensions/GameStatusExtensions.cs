using DevilDaggersInfo.Tools.GameMemory;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class GameStatusExtensions
{
	public static string ToDisplayString(this GameStatus gameStatus)
	{
		return gameStatus switch
		{
			GameStatus.Title => "Title",
			GameStatus.Menu => "Menu",
			GameStatus.Lobby => "Lobby",
			GameStatus.Playing => "Playing",
			GameStatus.Dead => "Dead",
			GameStatus.OwnReplayFromLastRun => "Own replay from last run",
			GameStatus.OwnReplayFromLeaderboard => "Own replay from leaderboard",
			GameStatus.OtherPlayersReplayFromLeaderboard => "Other player's replay from leaderboard",
			GameStatus.LocalReplay => "Local replay",
			_ => throw new UnreachableException(),
		};
	}
}
