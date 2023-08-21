using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.CriteriaExpression.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards.Results;

// TODO: Rewrite to use discriminated unions if they ever get added to C#.
public class UploadResult
{
	private const int _columnWidth = 120;
	private const int _headerWidth = _columnWidth * 3;
	private const int _indentation = 12;

	private static readonly Vector2 _iconSize = new(16);

	private readonly bool _isAscending;
	private readonly string _spawnsetName;
	private readonly Death? _death;
	private readonly GetUploadResponseFirstScore? _firstScore;
	private readonly GetUploadResponseHighscore? _highscore;
	private readonly GetUploadResponseNoHighscore? _noHighscore;
	private readonly GetUploadResponseCriteriaRejection? _criteriaRejection;

	public UploadResult(GetUploadResponse uploadResponse, bool isAscending, string spawnsetName, byte deathType, DateTime submittedAt)
	{
		if (uploadResponse.FirstScore != null)
			_firstScore = uploadResponse.FirstScore;
		else if (uploadResponse.Highscore != null)
			_highscore = uploadResponse.Highscore;
		else if (uploadResponse.NoHighscore != null)
			_noHighscore = uploadResponse.NoHighscore;
		else if (uploadResponse.CriteriaRejection != null)
			_criteriaRejection = uploadResponse.CriteriaRejection;
		else
			throw new InvalidOperationException("Invalid upload response returned from server.");

		_isAscending = isAscending;
		_spawnsetName = spawnsetName;
		_death = Deaths.GetDeathByLeaderboardType(GameConstants.CurrentVersion, deathType);
		SubmittedAt = submittedAt;
	}

	public DateTime SubmittedAt { get; }
	public bool IsExpanded { get; set; } = true;

	public void Render()
	{
		if (_firstScore != null)
			RenderFirstScore(_firstScore);
		else if (_highscore != null)
			RenderHighscore(_highscore);
		else if (_noHighscore != null)
			RenderNoHighscore(_noHighscore);
		else if (_criteriaRejection != null)
			RenderCriteriaRejection(_criteriaRejection);
		else
			throw new UnreachableException();
	}

	private void RenderFirstScore(GetUploadResponseFirstScore firstScore)
	{
		if (!RenderHeader(Color.Aqua, "First score!"))
			return;

		ImGui.Indent(_indentation);

		ImGui.Spacing();
		ImGuiImage.Image(Root.InternalResources.IconEyeTexture.Handle, _iconSize, Color.Orange);

		if (ImGui.BeginTable("Player", 2))
		{
			ConfigureColumns(2);

			Add("Rank", firstScore.Rank, static i => i.ToString());
			Add("Time", firstScore.Time, static d => d.ToString(StringFormats.TimeFormat));
			Add("Level 2", firstScore.LevelUpTime2, static i => i.ToString(StringFormats.TimeFormat));
			Add("Level 3", firstScore.LevelUpTime3, static i => i.ToString(StringFormats.TimeFormat));
			Add("Level 4", firstScore.LevelUpTime4, static i => i.ToString(StringFormats.TimeFormat));
			AddDeath(_death);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskGemTexture.Handle, _iconSize, Color.Red);

		if (ImGui.BeginTable("Gems", 2))
		{
			ConfigureColumns(2);

			Add("Gems collected", firstScore.GemsCollected, static i => i.ToString());
			Add("Gems despawned", firstScore.GemsDespawned, static i => i.ToString());
			Add("Gems eaten", firstScore.GemsEaten, static i => i.ToString());
			Add("Gems total", firstScore.GemsTotal, static i => i.ToString());

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskHomingTexture.Handle, _iconSize);

		if (ImGui.BeginTable("Homing", 2))
		{
			ConfigureColumns(2);

			Add("Homing stored", firstScore.HomingStored, static i => i.ToString());
			Add("Homing eaten", firstScore.HomingEaten, static i => i.ToString());

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskCrosshairTexture.Handle, _iconSize, Color.Green);

		if (ImGui.BeginTable("Daggers", 2))
		{
			ConfigureColumns(2);

			Add("Daggers fired", firstScore.DaggersFired, static i => i.ToString());
			Add("Daggers hit", firstScore.DaggersHit, static i => i.ToString());

			double accuracy = firstScore.DaggersFired == 0 ? 0 : firstScore.DaggersHit / (double)firstScore.DaggersFired;
			Add("Accuracy", accuracy, i => i.ToString(StringFormats.AccuracyFormat));

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskSkullTexture.Handle, _iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());

		if (ImGui.BeginTable("Enemies", 2))
		{
			ConfigureColumns(2);

			Add("Enemies killed", firstScore.EnemiesKilled, static i => i.ToString());
			Add("Enemies alive", firstScore.EnemiesAlive, static i => i.ToString());

			ImGui.EndTable();
		}

		ImGui.Indent(-_indentation);

		static void Add<T>(string label, T value, Func<T, string> formatter)
			where T : struct
		{
			ImGui.TableNextColumn();
			ImGui.Text(label);

			ImGui.TableNextColumn();
			ImGui.TextUnformatted(formatter(value));
		}
	}

	private void RenderHighscore(GetUploadResponseHighscore highscore)
	{
		if (!RenderHeader(Color.Green, "NEW HIGHSCORE!"))
			return;

		ImGui.Indent(_indentation);
		AddStates(
			_isAscending,
			highscore.RankState,
			highscore.TimeState,
			highscore.LevelUpTime2State,
			highscore.LevelUpTime3State,
			highscore.LevelUpTime4State,
			highscore.EnemiesKilledState,
			highscore.EnemiesAliveState,
			highscore.GemsCollectedState,
			highscore.GemsDespawnedState,
			highscore.GemsEatenState,
			highscore.GemsTotalState,
			highscore.HomingStoredState,
			highscore.HomingEatenState,
			highscore.DaggersFiredState,
			highscore.DaggersHitState,
			_death);
		ImGui.Indent(-_indentation);
	}

	private void RenderNoHighscore(GetUploadResponseNoHighscore noHighscore)
	{
		if (!RenderHeader(Color.White, "No new highscore."))
			return;

		ImGui.Indent(_indentation);
		AddStates(
			_isAscending,
			null,
			noHighscore.TimeState,
			noHighscore.LevelUpTime2State,
			noHighscore.LevelUpTime3State,
			noHighscore.LevelUpTime4State,
			noHighscore.EnemiesKilledState,
			noHighscore.EnemiesAliveState,
			noHighscore.GemsCollectedState,
			noHighscore.GemsDespawnedState,
			noHighscore.GemsEatenState,
			noHighscore.GemsTotalState,
			noHighscore.HomingStoredState,
			noHighscore.HomingEatenState,
			noHighscore.DaggersFiredState,
			noHighscore.DaggersHitState,
			_death);
		ImGui.Indent(-_indentation);
	}

	private void RenderCriteriaRejection(GetUploadResponseCriteriaRejection criteriaRejection)
	{
		if (!RenderHeader(Color.Red, "Rejected score."))
			return;

		ImGui.Indent(_indentation);

		ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + _headerWidth);
		ImGui.Text($"Run was rejected because the {criteriaRejection.CriteriaName} value was {criteriaRejection.ActualValue}.");
		ImGui.Text($"It must be {criteriaRejection.CriteriaOperator.ToCore().Display()} {criteriaRejection.ExpectedValue} in order to submit to this leaderboard.");
		ImGui.PopTextWrapPos();

		ImGui.Indent(-_indentation);
	}

	private bool RenderHeader(Color color, string title)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = 32 });
		if (ImGui.BeginChild(SubmittedAt.Ticks + _spawnsetName, new(_headerWidth, 48), true))
		{
			bool hover = ImGui.IsWindowHovered();
			if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				IsExpanded = !IsExpanded;

			ImGui.TextColored(color, _spawnsetName);

			string timeAgo = SubmittedAt.ToString("HH:mm:ss");
			ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.CalcTextSize(timeAgo).X - 8);
			ImGui.Text(timeAgo);

			ImGui.Text(title);
		}

		ImGui.EndChild(); // End {SubmittedAt.Ticks}{_spawnsetName}
		ImGui.PopStyleColor();

		return IsExpanded;
	}

	private static void AddStates(
		bool isAscending,
		GetScoreState<int>? rankState,
		GetScoreState<double> timeState,
		GetScoreState<double> levelUpTime2State,
		GetScoreState<double> levelUpTime3State,
		GetScoreState<double> levelUpTime4State,
		GetScoreState<int> enemiesKilledState,
		GetScoreState<int> enemiesAliveState,
		GetScoreState<int> gemsCollectedState,
		GetScoreState<int> gemsDespawnedState,
		GetScoreState<int> gemsEatenState,
		GetScoreState<int> gemsTotalState,
		GetScoreState<int> homingStoredState,
		GetScoreState<int> homingEatenState,
		GetScoreState<int> daggersFiredState,
		GetScoreState<int> daggersHitState,
		Death? death)
	{
		ImGui.Spacing();
		ImGuiImage.Image(Root.InternalResources.IconEyeTexture.Handle, _iconSize, Color.Orange);

		if (ImGui.BeginTable("Player", 3))
		{
			ConfigureColumns(3);

			if (rankState.HasValue)
				AddScoreState("Rank", rankState.Value, i => i.ToString(), i => $"{i:+0;-0;+0}");
			AddScoreState("Time", timeState, d => d.ToString(StringFormats.TimeFormat), i => $"{i:+0.0000;-0.0000;+0.0000}", !isAscending);
			AddLevelUpScoreState("Level 2", levelUpTime2State);
			AddLevelUpScoreState("Level 3", levelUpTime3State);
			AddLevelUpScoreState("Level 4", levelUpTime4State);
			AddDeath(death);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskGemTexture.Handle, _iconSize, Color.Red);

		if (ImGui.BeginTable("Gems", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Gems collected", gemsCollectedState, i => i.ToString(), i => $"{i:+0;-0;+0}");
			AddScoreState("Gems despawned", gemsDespawnedState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
			AddScoreState("Gems eaten", gemsEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
			AddScoreState("Gems total", gemsTotalState, i => i.ToString(), i => $"{i:+0;-0;+0}");

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskHomingTexture.Handle, _iconSize);

		if (ImGui.BeginTable("Homing", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Homing stored", homingStoredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
			AddScoreState("Homing eaten", homingEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskCrosshairTexture.Handle, _iconSize, Color.Green);

		if (ImGui.BeginTable("Daggers", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Daggers fired", daggersFiredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
			AddScoreState("Daggers hit", daggersHitState, i => i.ToString(), i => $"{i:+0;-0;+0}");

			double accuracy = GetAccuracy(daggersFiredState.Value, daggersHitState.Value);
			double oldAccuracy = GetAccuracy(daggersFiredState.Value - daggersFiredState.ValueDifference, daggersHitState.Value - daggersHitState.ValueDifference);
			GetScoreState<double> accuracyState = new()
			{
				Value = accuracy,
				ValueDifference = accuracy - oldAccuracy,
			};
			AddScoreState("Accuracy", accuracyState, i => i.ToString(StringFormats.AccuracyFormat), i => $"{i:+0.00%;-0.00%;+0.00%}");

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskSkullTexture.Handle, _iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());

		if (ImGui.BeginTable("Enemies", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Enemies killed", enemiesKilledState, i => i.ToString(), i => $"{i:+0;-0;+0}");
			AddScoreState("Enemies alive", enemiesAliveState, i => i.ToString(), i => $"{i:+0;-0;+0}");

			ImGui.EndTable();
		}
	}

	private static void AddScoreState<T>(string label, GetScoreState<T> scoreState, Func<T, string> formatter, Func<T, string> formatterDifference, bool higherIsBetter = true)
		where T : struct, INumber<T>
	{
		int comparison = scoreState.ValueDifference.CompareTo(T.Zero);
		if (!higherIsBetter)
			comparison = -comparison;
		Color color = comparison switch
		{
			-1 => Color.Red,
			0 => Color.White,
			1 => Color.Green,
			_ => throw new UnreachableException(),
		};

		ImGui.TableNextColumn();
		ImGui.Text(label);

		ImGui.TableNextColumn();
		ImGui.TextUnformatted(formatter(scoreState.Value));

		ImGui.TableNextColumn();
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.TextUnformatted(formatterDifference(scoreState.ValueDifference));
		ImGui.PopStyleColor();
	}

	private static void AddLevelUpScoreState(string label, GetScoreState<double> scoreState)
	{
		int comparison = -scoreState.ValueDifference.CompareTo(0);
		Color color = comparison switch
		{
			-1 => Color.Red,
			0 => Color.White,
			1 => Color.Green,
			_ => throw new UnreachableException(),
		};

		const double tolerance = 0.000001;

		// Show N/A for level up time when level was not achieved, for both value and difference.
		// Also, if the previous level up time was 0, but the level was achieved this time, the difference is equal to the value. Show N/A in this case as well.
		bool levelWasNotAchieved = scoreState.Value <= tolerance;
		bool diffIsSameAsValue = Math.Abs(scoreState.Value - scoreState.ValueDifference) <= tolerance;
		bool hideDifference = levelWasNotAchieved || diffIsSameAsValue;

		string levelUp = levelWasNotAchieved ? "N/A" : scoreState.Value.ToString(StringFormats.TimeFormat);
		string levelUpDifference = hideDifference ? "N/A" : scoreState.ValueDifference.ToString("+0.0000;-0.0000;+0.0000");

		ImGui.TableNextColumn();
		ImGui.Text(label);

		ImGui.TableNextColumn();
		ImGui.Text(levelUp);

		ImGui.TableNextColumn();
		ImGui.TextColored(hideDifference ? Color.White : color, levelUpDifference);
	}

	private static void AddDeath(Death? death)
	{
		ImGui.TableNextColumn();
		ImGui.Text("Death");

		ImGui.TableNextColumn();
		ImGui.TextColored(death?.Color.ToEngineColor() ?? Color.White, death?.Name ?? "Unknown");
	}

	private static void ConfigureColumns(int count)
	{
		for (int i = 0; i < count; i++)
			ImGui.TableSetupColumn(null, ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoHeaderLabel, _columnWidth);
	}

	private static double GetAccuracy(int daggersFired, int daggersHit)
		=> daggersFired == 0 ? 0 : daggersHit / (double)daggersFired;
}
