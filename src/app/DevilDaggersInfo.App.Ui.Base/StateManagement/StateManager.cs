using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.States;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.States;
using DevilDaggersInfo.App.Ui.Base.StateManagement.ReplayEditor.States;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.States;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement;

public static class StateManager
{
	private static readonly Dictionary<Type, List<IAction>> _actionsToReduce = new();
	private static readonly Dictionary<Type, List<Action>> _eventHandlers = new();

	// Base states.
	public static LayoutState LayoutState { get; private set; } = LayoutState.GetDefault();

	// Custom leaderboards recorder states.
	public static ActiveSpawnsetState ActiveSpawnsetState { get; private set; } = ActiveSpawnsetState.GetDefault();
	public static LeaderboardListState LeaderboardListState { get; private set; } = LeaderboardListState.GetDefault();
	public static MarkerState MarkerState { get; private set; } = MarkerState.GetDefault();
	public static RecordingState RecordingState { get; private set; } = RecordingState.GetDefault();
	public static ReplaySceneState ReplaySceneState { get; private set; } = ReplaySceneState.GetDefault();
	public static UploadResponseState UploadResponseState { get; private set; } = UploadResponseState.GetDefault();

	// Survival editor states.
	public static ArenaEditorState ArenaEditorState { get; private set; } = ArenaEditorState.GetDefault();
	public static ArenaPencilState ArenaPencilState { get; private set; } = ArenaPencilState.GetDefault();
	public static ArenaLineState ArenaLineState { get; private set; } = ArenaLineState.GetDefault();
	public static ArenaRectangleState ArenaRectangleState { get; private set; } = ArenaRectangleState.GetDefault();
	public static ArenaEllipseState ArenaEllipseState { get; private set; } = ArenaEllipseState.GetDefault();
	public static ArenaBucketState ArenaBucketState { get; private set; } = ArenaBucketState.GetDefault();
	public static ArenaDaggerState ArenaDaggerState { get; private set; } = ArenaDaggerState.GetDefault();
	public static SpawnEditorState SpawnEditorState { get; private set; } = SpawnEditorState.GetDefault();
	public static SpawnsetHistoryState SpawnsetHistoryState { get; private set; } = SpawnsetHistoryState.GetDefault();
	public static SpawnsetState SpawnsetState { get; private set; } = SpawnsetState.GetDefault();

	// Replay editor states.
	public static ReplayState ReplayState { get; private set; } = ReplayState.GetDefault();

	/// <summary>
	/// <para>
	/// Subscribes this <paramref name="eventHandler"/> to the specified <typeparamref name="TAction"/>.
	/// </para>
	/// <para>
	/// When one or more instances of this action type are dispatched, the event handler will be called <b>once</b> when all the actions are reduced.
	/// </para>
	/// <para>
	/// If this event handler is subscribed to multiple action types which are reduced at the same time, the event handler will still only be called once.
	/// </para>
	/// <para>
	/// This is done to prevent the event handler from executing twice, which can lead to corrupted components or crashes. Note that <see cref="NestingContext"/> also does not allow the same components to be added or removed more than once per update.
	/// </para>
	/// </summary>
	/// <param name="eventHandler">The event handler.</param>
	/// <typeparam name="TAction">The action type to subscribe to.</typeparam>
	public static void Subscribe<TAction>(Action eventHandler)
		where TAction : class, IAction
	{
		if (!_eventHandlers.ContainsKey(typeof(TAction)))
			_eventHandlers.Add(typeof(TAction), new());

		_eventHandlers[typeof(TAction)].Add(eventHandler);
	}

	/// <summary>
	/// Dispatches an action.
	/// </summary>
	/// <param name="action">The instance of the action.</param>
	/// <typeparam name="TAction">The type of the action.</typeparam>
	public static void Dispatch<TAction>(TAction action)
		where TAction : class, IAction
	{
		if (!_actionsToReduce.ContainsKey(typeof(TAction)))
			_actionsToReduce.Add(typeof(TAction), new());

		_actionsToReduce[typeof(TAction)].Add(action);
	}

	public static void ReduceAll()
	{
		// Copy the actions, because new actions can be dispatched while reducing.
		List<KeyValuePair<Type, List<IAction>>> copiedActionsToReduce = _actionsToReduce.ToList();
		_actionsToReduce.Clear();

		// Collect all the event handlers.
		List<Action> allEventHandlers = new();
		foreach (KeyValuePair<Type, List<IAction>> kvp in copiedActionsToReduce)
		{
			// Reduce the actions first.
			foreach (IAction action in kvp.Value)
			{
				StateReducer stateReducer = new();
				action.Reduce(stateReducer);

				LayoutState = stateReducer.LayoutState;
				ActiveSpawnsetState = stateReducer.ActiveSpawnsetState;
				LeaderboardListState = stateReducer.LeaderboardListState;
				MarkerState = stateReducer.MarkerState;
				RecordingState = stateReducer.RecordingState;
				ReplaySceneState = stateReducer.ReplaySceneState;
				UploadResponseState = stateReducer.UploadResponseState;
				ArenaEditorState = stateReducer.ArenaEditorState;
				ArenaPencilState = stateReducer.ArenaPencilState;
				ArenaLineState = stateReducer.ArenaLineState;
				ArenaRectangleState = stateReducer.ArenaRectangleState;
				ArenaEllipseState = stateReducer.ArenaEllipseState;
				ArenaBucketState = stateReducer.ArenaBucketState;
				ArenaDaggerState = stateReducer.ArenaDaggerState;
				SpawnEditorState = stateReducer.SpawnEditorState;
				SpawnsetHistoryState = stateReducer.SpawnsetHistoryState;
				SpawnsetState = stateReducer.SpawnsetState;
				ReplayState = stateReducer.ReplayState;
			}

			if (_eventHandlers.TryGetValue(kvp.Key, out List<Action>? eventHandlers))
			{
				// Add the event handlers unless it is already present. An event handler can only be executed once per update (for example, to avoid adding duplicate components).
				allEventHandlers.AddRange(eventHandlers.Where(h => !allEventHandlers.Contains(h)));
			}
		}

		// Execute all the event handlers.
		foreach (Action eventHandler in allEventHandlers)
			eventHandler.Invoke();
	}
}
