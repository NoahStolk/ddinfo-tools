using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public static class RecordingStateTypeExtensions
{
	public static string ToDisplayString(this RecordingStateType recordingStateType)
	{
		return recordingStateType switch
		{
			RecordingStateType.WaitingForGame => "Waiting for game",
			RecordingStateType.Recording => "Recording",
			RecordingStateType.WaitingForNextRun => "Waiting for next run",
			RecordingStateType.WaitingForLocalReplay => "Waiting for local replay",
			RecordingStateType.WaitingForLeaderboardReplay => "Waiting for lb replay",
			RecordingStateType.WaitingForStats => "Waiting for stats",
			RecordingStateType.WaitingForReplay => "Waiting for replay",
			RecordingStateType.Uploading => "Uploading",
			RecordingStateType.CompletedUpload => "Completed upload",
			_ => throw new UnreachableException(),
		};
	}
}
