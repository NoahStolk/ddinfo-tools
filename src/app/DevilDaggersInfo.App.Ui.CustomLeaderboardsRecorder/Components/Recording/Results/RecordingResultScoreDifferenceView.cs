using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;
using System.Diagnostics;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public abstract class RecordingResultScoreDifferenceView : RecordingResultScoreView
{
	protected RecordingResultScoreDifferenceView(IBounds bounds)
		: base(bounds)
	{
	}

	protected void AddStates(
		ref int y,
		bool isAscending,
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
		GetScoreState<int> daggersHitState)
	{
		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconEye, Color.Orange);
		AddScoreState(ref y, "Time", timeState, d => d.ToString(StringFormats.TimeFormat), i => $"{i:+0.0000;-0.0000;+0.0000}", !isAscending);
		AddLevelUpScoreState(ref y, "Level 2", levelUpTime2State);
		AddLevelUpScoreState(ref y, "Level 3", levelUpTime3State);
		AddLevelUpScoreState(ref y, "Level 4", levelUpTime4State);
		AddDeath(ref y);

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconGem, Color.Red);
		AddScoreState(ref y, "Gems collected", gemsCollectedState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		AddScoreState(ref y, "Gems despawned", gemsDespawnedState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
		AddScoreState(ref y, "Gems eaten", gemsEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
		AddScoreState(ref y, "Gems total", gemsTotalState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconHoming, Color.White);
		AddScoreState(ref y, "Homing stored", homingStoredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		AddScoreState(ref y, "Homing eaten", homingEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconCrosshair, Color.Green);
		AddScoreState(ref y, "Daggers fired", daggersFiredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		AddScoreState(ref y, "Daggers hit", daggersHitState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		static double GetAccuracy(int daggersFired, int daggersHit)
			=> daggersFired == 0 ? 0 : daggersHit / (double)daggersFired;

		double accuracy = GetAccuracy(daggersFiredState.Value, daggersHitState.Value);
		double oldAccuracy = GetAccuracy(daggersFiredState.Value - daggersFiredState.ValueDifference, daggersHitState.Value - daggersHitState.ValueDifference);
		GetScoreState<double> accuracyState = new()
		{
			Value = accuracy,
			ValueDifference = accuracy - oldAccuracy,
		};
		AddScoreState(ref y, "Accuracy", accuracyState, i => i.ToString(StringFormats.AccuracyFormat), i => $"{i:+0.00%;-0.00%;+0.00%}");

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconSkull, EnemiesV3_2.Skull4.Color.ToWarpColor());
		AddScoreState(ref y, "Enemies killed", enemiesKilledState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		AddScoreState(ref y, "Enemies alive", enemiesAliveState, i => i.ToString(), i => $"{i:+0;-0;+0}");
	}

	protected void AddScoreState<T>(ref int y, string label, GetScoreState<T> scoreState, Func<T, string> formatter, Func<T, string> formatterDifference, bool higherIsBetter = true)
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

		int columnWidth = Bounds.Size.X / 3;

		Label l = new(Bounds.CreateNested(columnWidth * 0, y, columnWidth, 16), label, LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		Label m = new(Bounds.CreateNested(columnWidth * 1, y, columnWidth, 16), formatter(scoreState.Value), LabelStyles.DefaultRight) { Depth = Depth + 2 };
		Label r = new(Bounds.CreateNested(columnWidth * 2, y, columnWidth, 16), formatterDifference(scoreState.ValueDifference), LabelStyles.DefaultRight with { TextColor = color }) { Depth = Depth + 2 };
		NestingContext.Add(l);
		NestingContext.Add(m);
		NestingContext.Add(r);

		y += _labelHeight;
	}

	private void AddLevelUpScoreState(ref int y, string label, GetScoreState<double> scoreState)
	{
		int comparison = -scoreState.ValueDifference.CompareTo(0);
		Color color = comparison switch
		{
			-1 => Color.Red,
			0 => Color.White,
			1 => Color.Green,
			_ => throw new UnreachableException(),
		};

		int columnWidth = Bounds.Size.X / 3;

		const double tolerance = 0.000001;

		// Show N/A for level up time when level was not achieved, for both value and difference.
		// Also, if the previous level up time was 0, but the level was achieved this time, the difference is equal to the value. Show N/A in this case as well.
		bool levelWasNotAchieved = scoreState.Value <= tolerance;
		bool diffIsSameAsValue = Math.Abs(scoreState.Value - scoreState.ValueDifference) <= tolerance;
		bool hideDifference = levelWasNotAchieved || diffIsSameAsValue;

		string levelUp = levelWasNotAchieved ? "N/A" : scoreState.Value.ToString(StringFormats.TimeFormat);
		string levelUpDifference = hideDifference ? "N/A" : scoreState.ValueDifference.ToString("+0.0000;-0.0000;+0.0000");
		LabelStyle differenceStyle = hideDifference ? LabelStyles.DefaultRight : LabelStyles.DefaultRight with { TextColor = color };

		Label l = new(Bounds.CreateNested(columnWidth * 0, y, columnWidth, 16), label, LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		Label m = new(Bounds.CreateNested(columnWidth * 1, y, columnWidth, 16), levelUp, LabelStyles.DefaultRight) { Depth = Depth + 2 };
		Label r = new(Bounds.CreateNested(columnWidth * 2, y, columnWidth, 16), levelUpDifference, differenceStyle) { Depth = Depth + 2 };
		NestingContext.Add(l);
		NestingContext.Add(m);
		NestingContext.Add(r);

		y += _labelHeight;
	}
}
