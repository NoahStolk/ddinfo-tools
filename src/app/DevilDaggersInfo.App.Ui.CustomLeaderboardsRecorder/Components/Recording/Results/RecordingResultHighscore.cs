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

public class RecordingResultHighscore : RecordingResultScoreView
{
	public RecordingResultHighscore(IBounds bounds, GetUploadResponseHighscore response, bool isAscending)
		: base(bounds)
	{
		Label header = new(Bounds.CreateNested(0, 0, Bounds.Size.X, 16), "NEW HIGHSCORE!", LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		NestingContext.Add(header);

		int y = _yStart;
		Add("Rank", response.RankState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconEye, Color.Orange);
		Add("Time", response.TimeState, d => d.ToString(StringFormats.TimeFormat), i => $"{i:+0.0000;-0.0000;+0.0000}", !isAscending);
		AddLevelUp("Level 2", response.LevelUpTime2State);
		AddLevelUp("Level 3", response.LevelUpTime3State);
		AddLevelUp("Level 4", response.LevelUpTime4State);
		AddDeath(ref y);

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconGem, Color.Red);
		Add("Gems collected", response.GemsCollectedState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		Add("Gems despawned", response.GemsDespawnedState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
		Add("Gems eaten", response.GemsEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);
		Add("Gems total", response.GemsTotalState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconHoming, Color.White);
		Add("Homing stored", response.HomingStoredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		Add("Homing eaten", response.HomingEatenState, i => i.ToString(), i => $"{i:+0;-0;+0}", false);

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconCrosshair, Color.Green);
		Add("Daggers fired", response.DaggersFiredState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		Add("Daggers hit", response.DaggersHitState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		static double GetAccuracy(int daggersFired, int daggersHit)
			=> daggersFired == 0 ? 0 : daggersHit / (double)daggersFired;

		double accuracy = GetAccuracy(response.DaggersFiredState.Value, response.DaggersHitState.Value);
		double oldAccuracy = GetAccuracy(response.DaggersFiredState.Value - response.DaggersFiredState.ValueDifference, response.DaggersHitState.Value - response.DaggersHitState.ValueDifference);
		GetScoreState<double> accuracyState = new()
		{
			Value = accuracy,
			ValueDifference = accuracy - oldAccuracy,
		};
		Add("Accuracy", accuracyState, i => i.ToString(StringFormats.AccuracyFormat), i => $"{i:+0.00%;-0.00%;+0.00%}");

		AddSpacing(ref y);
		AddIcon(ref y, WarpTextures.IconSkull, EnemiesV3_2.Skull4.Color.ToWarpColor());
		Add("Enemies killed", response.EnemiesKilledState, i => i.ToString(), i => $"{i:+0;-0;+0}");
		Add("Enemies alive", response.EnemiesAliveState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		void Add<T>(string label, GetScoreState<T> scoreState, Func<T, string> formatter, Func<T, string> formatterDifference, bool higherIsBetter = true)
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

		void AddLevelUp(string label, GetScoreState<double> scoreState)
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
}
