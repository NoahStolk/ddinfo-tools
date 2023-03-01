namespace DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;

/// <summary>
/// Fires when the installation has been verified and DD content has been found.
/// This action should only fire once.
/// </summary>
public record InitializeContent : IAction
{
	public void Reduce(StateReducer stateReducer)
	{
	}
}
