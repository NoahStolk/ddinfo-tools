namespace DevilDaggersInfo.App.Ui.Base.StateManagement;

/// <summary>
/// Represents an action dispatched by the <see cref="StateManager"/>.
/// </summary>
public interface IAction
{
	/// <summary>
	/// Modifies state.
	/// </summary>
	void Reduce(StateReducer stateReducer);
}
