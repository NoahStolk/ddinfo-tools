using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Styling;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;

public class RecordingResultNoHighscore : RecordingResultScoreDifferenceView
{
	public RecordingResultNoHighscore(
		IBounds bounds,
		GetUploadResponseNoHighscore response,
		bool isAscending)
		: base(bounds)
	{
		Label header = new(Bounds.CreateNested(0, 0, Bounds.Size.X, 16), "No new highscore.", LabelStyles.DefaultLeft) { Depth = Depth + 2 };
		NestingContext.Add(header);

		int y = _yStart;
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
