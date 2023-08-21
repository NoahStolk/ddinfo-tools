// #define TESTING
#if TESTING
using DevilDaggersInfo.Api.App.CustomLeaderboards;
#endif
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards.Results;

public static class CustomLeaderboardResultsWindow
{
	private static readonly List<UploadResult> _results = new();

	public static void AddResult(UploadResult newResult)
	{
		foreach (UploadResult result in _results)
			result.IsExpanded = false;

		_results.Add(newResult);
	}

	public static void Render()
	{
		ImGui.SetNextWindowSizeConstraints(new(388, 0), new(388, float.MaxValue));
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 320));
		if (ImGui.Begin("Custom Leaderboard Submissions (this session)"))
		{
			ImGui.PopStyleVar();

#if TESTING
		if (ImGui.Button("Random first score"))
			AddRandomFirstScore();

		if (ImGui.Button("Random highscore"))
			AddRandomHighscore();

		if (ImGui.Button("Random no highscore"))
			AddRandomNoHighscore();

		if (ImGui.Button("Random rejection"))
			AddRandomRejection();

		if (ImGui.Button("Clear"))
			_results.Clear();
#endif

			if (ImGui.Button("Collapse all"))
				_results.ForEach(r => r.IsExpanded = false);

			ImGui.SameLine();
			if (ImGui.Button("Expand all"))
				_results.ForEach(r => r.IsExpanded = true);

			foreach (UploadResult result in _results.OrderByDescending(ur => ur.SubmittedAt))
				result.Render();
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Custom Leaderboard Submissions (this session)
	}

#if TESTING
	private static GetUploadResponse GetResponse()
	{
		return new()
		{
			IsAscending = true,
			SpawnsetId = Random.Shared.Next(0, 17),
			SpawnsetName = "TestSet",
			CustomLeaderboardId = Random.Shared.Next(0, 17),
			NewSortedEntries = new(),
		};
	}

	private static void AddRandomFirstScore()
	{
		GetUploadResponseFirstScore firstScore = new()
		{
			Rank = Random.Shared.Next(1, 10),
			Time = Random.Shared.NextSingle() * 500 + 10,
			DaggersFired = Random.Shared.Next(500, 1000),
			DaggersHit = Random.Shared.Next(400, 500),
			EnemiesAlive = Random.Shared.Next(0, 100),
			EnemiesKilled = Random.Shared.Next(0, 1000),
			GemsCollected = Random.Shared.Next(0, 100),
			GemsDespawned = Random.Shared.Next(0, 10),
			GemsEaten = Random.Shared.Next(0, 10),
			GemsTotal = Random.Shared.Next(0, 500),
			HomingEaten = Random.Shared.Next(0, 30),
			HomingStored = Random.Shared.Next(0, 300),
			LevelUpTime2 = Random.Shared.NextSingle() * 500 + 10,
			LevelUpTime3 = Random.Shared.NextSingle() * 500 + 10,
			LevelUpTime4 = Random.Shared.NextSingle() * 500 + 10,
		};
		AddResult(new(GetResponse() with { FirstScore = firstScore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.Now));
	}

	private static void AddRandomHighscore()
	{
		GetUploadResponseHighscore highscore = new()
		{
			RankState = new(Random.Shared.Next(1, 10), Random.Shared.Next(1, 10)),
			TimeState = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			DaggersFiredState = new(Random.Shared.Next(500, 1000), Random.Shared.Next(500, 1000)),
			DaggersHitState = new(Random.Shared.Next(400, 500), Random.Shared.Next(400, 500)),
			EnemiesAliveState = new(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			EnemiesKilledState = new(Random.Shared.Next(0, 1000), Random.Shared.Next(0, 1000)),
			GemsCollectedState = new(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			GemsDespawnedState = new(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsEatenState = new(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsTotalState = new(Random.Shared.Next(0, 500), Random.Shared.Next(0, 500)),
			HomingEatenState = new(Random.Shared.Next(0, 30), Random.Shared.Next(0, 30)),
			HomingStoredState = new(Random.Shared.Next(0, 300), Random.Shared.Next(0, 300)),
			LevelUpTime2State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime3State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime4State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
		};
		AddResult(new(GetResponse() with { Highscore = highscore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.Now));
	}

	private static void AddRandomNoHighscore()
	{
		GetUploadResponseNoHighscore noHighscore = new()
		{
			TimeState = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			DaggersFiredState = new(Random.Shared.Next(500, 1000), Random.Shared.Next(500, 1000)),
			DaggersHitState = new(Random.Shared.Next(400, 500), Random.Shared.Next(400, 500)),
			EnemiesAliveState = new(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			EnemiesKilledState = new(Random.Shared.Next(0, 1000), Random.Shared.Next(0, 1000)),
			GemsCollectedState = new(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			GemsDespawnedState = new(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsEatenState = new(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsTotalState = new(Random.Shared.Next(0, 500), Random.Shared.Next(0, 500)),
			HomingEatenState = new(Random.Shared.Next(0, 30), Random.Shared.Next(0, 30)),
			HomingStoredState = new(Random.Shared.Next(0, 300), Random.Shared.Next(0, 300)),
			LevelUpTime2State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime3State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime4State = new(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
		};
		AddResult(new(GetResponse() with { NoHighscore = noHighscore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.Now));
	}

	private static void AddRandomRejection()
	{
		GetUploadResponseCriteriaRejection rejection = new()
		{
			ActualValue = 5,
			CriteriaName = "kills",
			CriteriaOperator = CustomLeaderboardCriteriaOperator.GreaterThan,
			ExpectedValue = 6,
		};
		AddResult(new(GetResponse() with { CriteriaRejection = rejection }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.Now));
	}
#endif
}
