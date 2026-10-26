#define TESTING
#if TESTING
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
#endif
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;

internal sealed class CustomLeaderboardsResultsWindow(ResourceManager resourceManager, CustomLeaderboardsResultState customLeaderboardsResultState)
{
	public void Render()
	{
		ImGui.SetNextWindowSizeConstraints(new Vector2(388, 0), new Vector2(388, float.MaxValue));
		ImGuiUtils.SetNextWindowMinSize(0, 320);
		if (ImGui.Begin("Custom Leaderboard Submissions (this session)"))
		{
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
				customLeaderboardsResultState.ClearResults();
#endif

			if (ImGui.Button("Collapse all"))
				customLeaderboardsResultState.SetExpanded(false);

			ImGui.SameLine();
			if (ImGui.Button("Expand all"))
				customLeaderboardsResultState.SetExpanded(true);

			for (int i = 0; i < customLeaderboardsResultState.Count; i++)
			{
				UploadResult result = customLeaderboardsResultState[i];
				result.Render();
			}
		}

		ImGui.End();
	}

#if TESTING
	private static GetUploadResponse GetResponse()
	{
		return new GetUploadResponse
		{
			IsAscending = true,
			SpawnsetId = Random.Shared.Next(0, 17),
			SpawnsetName = "TestSet",
			CustomLeaderboardId = Random.Shared.Next(0, 17),
			NewSortedEntries = [],
		};
	}

	private void AddRandomFirstScore()
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
		customLeaderboardsResultState.AddResult(new UploadResult(resourceManager, GetResponse() with { FirstScore = firstScore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.UtcNow));
	}

	private void AddRandomHighscore()
	{
		GetUploadResponseHighscore highscore = new()
		{
			RankState = new GetScoreState<int>(Random.Shared.Next(1, 10), Random.Shared.Next(1, 10)),
			TimeState = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			DaggersFiredState = new GetScoreState<int>(Random.Shared.Next(500, 1000), Random.Shared.Next(500, 1000)),
			DaggersHitState = new GetScoreState<int>(Random.Shared.Next(400, 500), Random.Shared.Next(400, 500)),
			EnemiesAliveState = new GetScoreState<int>(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			EnemiesKilledState = new GetScoreState<int>(Random.Shared.Next(0, 1000), Random.Shared.Next(0, 1000)),
			GemsCollectedState = new GetScoreState<int>(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			GemsDespawnedState = new GetScoreState<int>(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsEatenState = new GetScoreState<int>(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsTotalState = new GetScoreState<int>(Random.Shared.Next(0, 500), Random.Shared.Next(0, 500)),
			HomingEatenState = new GetScoreState<int>(Random.Shared.Next(0, 30), Random.Shared.Next(0, 30)),
			HomingStoredState = new GetScoreState<int>(Random.Shared.Next(0, 300), Random.Shared.Next(0, 300)),
			LevelUpTime2State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime3State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime4State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
		};
		customLeaderboardsResultState.AddResult(new UploadResult(resourceManager, GetResponse() with { Highscore = highscore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.UtcNow));
	}

	private void AddRandomNoHighscore()
	{
		GetUploadResponseNoHighscore noHighscore = new()
		{
			TimeState = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			DaggersFiredState = new GetScoreState<int>(Random.Shared.Next(500, 1000), Random.Shared.Next(500, 1000)),
			DaggersHitState = new GetScoreState<int>(Random.Shared.Next(400, 500), Random.Shared.Next(400, 500)),
			EnemiesAliveState = new GetScoreState<int>(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			EnemiesKilledState = new GetScoreState<int>(Random.Shared.Next(0, 1000), Random.Shared.Next(0, 1000)),
			GemsCollectedState = new GetScoreState<int>(Random.Shared.Next(0, 100), Random.Shared.Next(0, 100)),
			GemsDespawnedState = new GetScoreState<int>(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsEatenState = new GetScoreState<int>(Random.Shared.Next(0, 10), Random.Shared.Next(0, 10)),
			GemsTotalState = new GetScoreState<int>(Random.Shared.Next(0, 500), Random.Shared.Next(0, 500)),
			HomingEatenState = new GetScoreState<int>(Random.Shared.Next(0, 30), Random.Shared.Next(0, 30)),
			HomingStoredState = new GetScoreState<int>(Random.Shared.Next(0, 300), Random.Shared.Next(0, 300)),
			LevelUpTime2State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime3State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
			LevelUpTime4State = new GetScoreState<double>(Random.Shared.NextSingle() * 500 + 10, Random.Shared.NextSingle() * 500 + 10),
		};
		customLeaderboardsResultState.AddResult(new UploadResult(resourceManager, GetResponse() with { NoHighscore = noHighscore }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.UtcNow));
	}

	private void AddRandomRejection()
	{
		GetUploadResponseCriteriaRejection rejection = new()
		{
			ActualValue = 5,
			CriteriaName = "kills",
			CriteriaOperator = CustomLeaderboardCriteriaOperator.GreaterThan,
			ExpectedValue = 6,
		};
		customLeaderboardsResultState.AddResult(new UploadResult(resourceManager, GetResponse() with { CriteriaRejection = rejection }, true, "TestSet", (byte)Random.Shared.Next(0, 17), DateTime.UtcNow));
	}
#endif
}
