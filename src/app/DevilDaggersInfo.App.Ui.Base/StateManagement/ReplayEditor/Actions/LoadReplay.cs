using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.ReplayEditor.Actions;

/// <summary>
/// Fires when a new replay is loaded from disk or elsewhere.
/// Do not use this when editing an existing replay in the UI.
/// This sets the replay name and clears the history.
/// </summary>
public record LoadReplay(string ReplayName, ReplayBinary<LocalReplayBinaryHeader> ReplayBinary) : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
		stateReducer.ReplayState = new(ReplayName, ReplayBinary);
	}
}
