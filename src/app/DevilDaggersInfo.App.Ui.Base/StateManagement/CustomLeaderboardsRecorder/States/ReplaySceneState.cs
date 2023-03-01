using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.States;

public record ReplaySceneState(ReplayBinary<LocalReplayBinaryHeader>[] ReplayBinaries)
{
	public static ReplaySceneState GetDefault()
	{
		return new(Array.Empty<ReplayBinary<LocalReplayBinaryHeader>>());
	}
}
