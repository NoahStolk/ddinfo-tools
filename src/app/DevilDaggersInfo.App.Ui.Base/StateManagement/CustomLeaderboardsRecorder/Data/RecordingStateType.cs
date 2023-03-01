namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;

public enum RecordingStateType
{
	WaitingForGame,
	Recording,
	WaitingForNextRun,
	WaitingForLocalReplay,
	WaitingForLeaderboardReplay,
	WaitingForStats,
	WaitingForReplay,
	Uploading,
	CompletedUpload,
}
