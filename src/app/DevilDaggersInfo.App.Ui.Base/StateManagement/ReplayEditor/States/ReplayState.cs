using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.ReplayEditor.States;

public record ReplayState(string ReplayName, ReplayBinary<LocalReplayBinaryHeader> Replay)
{
	public static ReplayState GetDefault()
	{
		return new("(untitled)", ReplayBinary<LocalReplayBinaryHeader>.CreateDefault());
	}
}
