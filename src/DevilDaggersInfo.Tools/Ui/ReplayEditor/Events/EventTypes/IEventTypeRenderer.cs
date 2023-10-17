using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public interface IEventTypeRenderer<in T>
{
	static abstract int ColumnCount { get; }

	static abstract int ColumnCountData { get; }

	static abstract void SetupColumns();

	static abstract void SetupColumnsData();

	static abstract void Render(int eventIndex, int entityId, T e, ReplayEventsData replayEventsData);

	static abstract void RenderData(int eventIndex, T e, ReplayEventsData replayEventsData);
}
