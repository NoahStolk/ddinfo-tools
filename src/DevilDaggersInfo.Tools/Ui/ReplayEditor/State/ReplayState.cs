using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.State;

public static class ReplayState
{
	public static ReplayBinary<LocalReplayBinaryHeader> Replay { get; set; } = ReplayBinary<LocalReplayBinaryHeader>.CreateDefault();

	public static string ReplayName { get; set; } = "(untitled)";
}