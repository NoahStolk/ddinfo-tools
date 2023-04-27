using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public class RecordingResultFirstScore : RecordingResultScoreView
{
	public RecordingResultFirstScore(IBounds bounds, GetUploadResponseFirstScore response)
		: base(bounds)
	{
		Label header = new(Bounds.CreateNested(0, 0, Bounds.Size.X, 16), "First score!", LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		NestingContext.Add(header);

		int y = _yStart;
		Add("Rank", response.Rank, i => i.ToString());

		AddSpacing(ref y);
		AddIcon(ref y, Textures.IconEye, Color.Orange);
		Add("Time", response.Time, d => d.ToString(StringFormats.TimeFormat));
		Add("Level 2", response.LevelUpTime2, i => i.ToString(StringFormats.TimeFormat));
		Add("Level 3", response.LevelUpTime3, i => i.ToString(StringFormats.TimeFormat));
		Add("Level 4", response.LevelUpTime4, i => i.ToString(StringFormats.TimeFormat));
		AddDeath(ref y);

		AddSpacing(ref y);
		AddIcon(ref y, Textures.IconGem, Color.Red);
		Add("Gems collected", response.GemsCollected, i => i.ToString());
		Add("Gems despawned", response.GemsDespawned, i => i.ToString());
		Add("Gems eaten", response.GemsEaten, i => i.ToString());
		Add("Gems total", response.GemsTotal, i => i.ToString());

		AddSpacing(ref y);
		AddIcon(ref y, Textures.IconHoming, Color.White);
		Add("Homing stored", response.HomingStored, i => i.ToString());
		Add("Homing eaten", response.HomingEaten, i => i.ToString());

		AddSpacing(ref y);
		AddIcon(ref y, Textures.IconCrosshair, Color.Green);
		Add("Daggers fired", response.DaggersFired, i => i.ToString());
		Add("Daggers hit", response.DaggersHit, i => i.ToString());

		double accuracy = response.DaggersFired == 0 ? 0 : response.DaggersHit / (double)response.DaggersFired;
		Add("Accuracy", accuracy, i => i.ToString(StringFormats.AccuracyFormat));

		AddSpacing(ref y);
		AddIcon(ref y, Textures.IconSkull, EnemiesV3_2.Skull4.Color.ToEngineColor());
		Add("Enemies killed", response.EnemiesKilled, i => i.ToString());
		Add("Enemies alive", response.EnemiesAlive, i => i.ToString());

		void Add<T>(string label, T value, Func<T, string> formatter)
			where T : struct
		{
			Label left = new(Bounds.CreateNested(0, y, Bounds.Size.X / 2, _labelHeight), label, LabelStyles.DefaultLeft) { Depth = Depth + 2 };
			Label right = new(Bounds.CreateNested(Bounds.Size.X / 2, y, Bounds.Size.X / 2, _labelHeight), formatter(value), LabelStyles.DefaultRight) { Depth = Depth + 2 };
			NestingContext.Add(left);
			NestingContext.Add(right);

			y += _labelHeight;
		}
	}
}
