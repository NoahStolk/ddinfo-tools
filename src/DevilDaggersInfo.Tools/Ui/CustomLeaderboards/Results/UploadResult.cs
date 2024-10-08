using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.CriteriaExpression.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Utils;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;

public class UploadResult
{
	private const int _columnWidth = 120;
	private const int _headerWidth = _columnWidth * 3;
	private const int _indentation = 12;

	private static readonly Vector2 _iconSize = new(16);

	private readonly bool _isAscending;
	private readonly string _spawnsetName;
	private readonly Death? _death;

	// TODO: Rewrite to use discriminated unions if they ever get added to C#.
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
		_death = Deaths.GetDeathByType(GameConstants.CurrentVersion, deathType);
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
		ImGuiImage.Image(Root.InternalResources.IconEyeTexture.Id, _iconSize, Color.Orange);

		if (ImGui.BeginTable("Player", 2))
		{
			ConfigureColumns(2);

			Add("Rank", Inline.Span(firstScore.Rank));
			Add("Time", Inline.Span(firstScore.Time, StringFormats.TimeFormat));
			Add("Level 2", Inline.Span(firstScore.LevelUpTime2, StringFormats.TimeFormat));
			Add("Level 3", Inline.Span(firstScore.LevelUpTime3, StringFormats.TimeFormat));
			Add("Level 4", Inline.Span(firstScore.LevelUpTime4, StringFormats.TimeFormat));
			AddDeath(_death);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskGemTexture.Id, _iconSize, Color.Red);

		if (ImGui.BeginTable("Gems", 2))
		{
			ConfigureColumns(2);

			Add("Gems collected", Inline.Span(firstScore.GemsCollected));
			Add("Gems despawned", Inline.Span(firstScore.GemsDespawned));
			Add("Gems eaten", Inline.Span(firstScore.GemsEaten));
			Add("Gems total", Inline.Span(firstScore.GemsTotal));

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskHomingTexture.Id, _iconSize);

		if (ImGui.BeginTable("Homing", 2))
		{
			ConfigureColumns(2);

			Add("Homing stored", Inline.Span(firstScore.HomingStored));
			Add("Homing eaten", Inline.Span(firstScore.HomingEaten));

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskCrosshairTexture.Id, _iconSize, Color.Green);

		if (ImGui.BeginTable("Daggers", 2))
		{
			ConfigureColumns(2);

			Add("Daggers fired", Inline.Span(firstScore.DaggersFired));
			Add("Daggers hit", Inline.Span(firstScore.DaggersHit));

			double accuracy = firstScore.DaggersFired == 0 ? 0 : firstScore.DaggersHit / (double)firstScore.DaggersFired;
			Add("Accuracy", Inline.Span(accuracy, StringFormats.AccuracyFormat));

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskSkullTexture.Id, _iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());

		if (ImGui.BeginTable("Enemies", 2))
		{
			ConfigureColumns(2);

			Add("Enemies killed", Inline.Span(firstScore.EnemiesKilled));
			Add("Enemies alive", Inline.Span(firstScore.EnemiesAlive));

			ImGui.EndTable();
		}

		ImGui.Indent(-_indentation);

		static void Add(string label, ReadOnlySpan<char> value)
		{
			ImGui.TableNextColumn();
			ImGui.Text(label);

			ImGui.TableNextColumn();
			ImGui.TextUnformatted(value);
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
		ImGui.Text(Inline.Span($"Run was rejected because the {criteriaRejection.CriteriaName} value was {criteriaRejection.ActualValue}."));
		ImGui.Text(Inline.Span($"It must be {criteriaRejection.CriteriaOperator.ToCore().Display()} {criteriaRejection.ExpectedValue} in order to submit to this leaderboard."));
		ImGui.PopTextWrapPos();

		ImGui.Indent(-_indentation);
	}

	private bool RenderHeader(Color color, string title)
	{
		ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = 32 });

		if (ImGui.BeginChild(Inline.Span($"{SubmittedAt.Ticks}{_spawnsetName}"), new Vector2(_headerWidth, 48), ImGuiChildFlags.Border))
		{
			bool hover = ImGui.IsWindowHovered();
			if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				IsExpanded = !IsExpanded;

			ImGui.TextColored(color, _spawnsetName);

			// TODO: Use ReadOnlySpan<char>.
			string text = DateTimeUtils.FormatTimeAgo(SubmittedAt);
			ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.CalcTextSize(text).X - 8);
			ImGui.Text(text);

			ImGui.Text(title);
		}

		ImGui.EndChild();
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
		ImGuiImage.Image(Root.InternalResources.IconEyeTexture.Id, _iconSize, Color.Orange);

		if (ImGui.BeginTable("Player", 3))
		{
			ConfigureColumns(3);

			if (rankState.HasValue)
				AddScoreState("Rank", rankState.Value, "0", "+0;-0;+0");
			AddScoreState("Time", timeState, StringFormats.TimeFormat, "+0.0000;-0.0000;+0.0000", !isAscending);
			AddLevelUpScoreState("Level 2", levelUpTime2State);
			AddLevelUpScoreState("Level 3", levelUpTime3State);
			AddLevelUpScoreState("Level 4", levelUpTime4State);
			AddDeath(death);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskGemTexture.Id, _iconSize, Color.Red);

		if (ImGui.BeginTable("Gems", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Gems collected", gemsCollectedState, "0", "+0;-0;+0");
			AddScoreState("Gems despawned", gemsDespawnedState, "0", "+0;-0;+0", false);
			AddScoreState("Gems eaten", gemsEatenState, "0", "+0;-0;+0", false);
			AddScoreState("Gems total", gemsTotalState, "0", "+0;-0;+0");

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskHomingTexture.Id, _iconSize);

		if (ImGui.BeginTable("Homing", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Homing stored", homingStoredState, "0", "+0;-0;+0");
			AddScoreState("Homing eaten", homingEatenState, "0", "+0;-0;+0", false);

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskCrosshairTexture.Id, _iconSize, Color.Green);

		if (ImGui.BeginTable("Daggers", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Daggers fired", daggersFiredState, "0", "+0;-0;+0");
			AddScoreState("Daggers hit", daggersHitState, "0", "+0;-0;+0");

			double accuracy = GetAccuracy(daggersFiredState.Value, daggersHitState.Value);
			double oldAccuracy = GetAccuracy(daggersFiredState.Value - daggersFiredState.ValueDifference, daggersHitState.Value - daggersHitState.ValueDifference);
			GetScoreState<double> accuracyState = new()
			{
				Value = accuracy,
				ValueDifference = accuracy - oldAccuracy,
			};
			AddScoreState("Accuracy", accuracyState, StringFormats.AccuracyFormat, "+0.00%;-0.00%;+0.00%");

			ImGui.EndTable();
		}

		ImGui.Spacing();
		ImGuiImage.Image(Root.GameResources.IconMaskSkullTexture.Id, _iconSize, EnemiesV3_2.Skull4.Color.ToEngineColor());

		if (ImGui.BeginTable("Enemies", 3))
		{
			ConfigureColumns(3);

			AddScoreState("Enemies killed", enemiesKilledState, "0", "+0;-0;+0");
			AddScoreState("Enemies alive", enemiesAliveState, "0", "+0;-0;+0");

			ImGui.EndTable();
		}
	}

	private static void AddScoreState<T>(ReadOnlySpan<char> label, GetScoreState<T> scoreState, ReadOnlySpan<char> format, ReadOnlySpan<char> formatDifference, bool higherIsBetter = true)
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
		ImGui.TextUnformatted(Inline.Span(scoreState.Value, format));

		ImGui.TableNextColumn();
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.TextUnformatted(Inline.Span(scoreState.ValueDifference, formatDifference));
		ImGui.PopStyleColor();
	}

	private static void AddLevelUpScoreState(ReadOnlySpan<char> label, GetScoreState<double> scoreState)
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

		ImGui.TableNextColumn();
		ImGui.Text(label);

		ImGui.TableNextColumn();
		ImGui.Text(levelWasNotAchieved ? "N/A" : Inline.Span(scoreState.Value, StringFormats.TimeFormat));

		ImGui.TableNextColumn();
		ImGui.TextColored(hideDifference ? Color.White : color, hideDifference ? "N/A" : Inline.Span(scoreState.ValueDifference, "+0.0000;-0.0000;+0.0000"));
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
	{
		return daggersFired == 0 ? 0 : daggersHit / (double)daggersFired;
	}
}
