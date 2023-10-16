using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public interface IEventTypeRenderer<in T>
{
	static abstract IReadOnlyList<EventColumn> EventColumns { get; }

	static abstract void Render(int eventIndex, int entityId, T e, ReplayEventsData replayEventsData);
}
