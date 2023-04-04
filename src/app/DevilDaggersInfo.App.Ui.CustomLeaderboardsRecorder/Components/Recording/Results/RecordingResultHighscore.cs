using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public class RecordingResultHighscore : RecordingResultScoreDifferenceView
{
	public RecordingResultHighscore(
		IBounds bounds,
		GetUploadResponseHighscore response,
		bool isAscending)
		: base(bounds)
	{
		Label header = new(Bounds.CreateNested(0, 0, Bounds.Size.X, 16), "NEW HIGHSCORE!", LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		NestingContext.Add(header);

		int y = _yStart;
		AddScoreState(ref y, "Rank", response.RankState, i => i.ToString(), i => $"{i:+0;-0;+0}");

		AddStates(
			ref y,
			isAscending,
			response.TimeState,
			response.LevelUpTime2State,
			response.LevelUpTime3State,
			response.LevelUpTime4State,
			response.EnemiesKilledState,
			response.EnemiesAliveState,
			response.GemsCollectedState,
			response.GemsDespawnedState,
			response.GemsEatenState,
			response.GemsTotalState,
			response.HomingStoredState,
			response.HomingEatenState,
			response.DaggersFiredState,
			response.DaggersHitState);
	}
}
