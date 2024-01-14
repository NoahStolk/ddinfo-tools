using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class GemEvents : IEventTypeRenderer<GemEventData>
{
	public static int ColumnCount => 2;
	public static int ColumnCountData => 0;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
	}

	public static void Render(int eventIndex, int entityId, GemEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, GemEventData e, ReplayEventsData replayEventsData)
	{
	}

	public static void RenderEdit(int eventIndex, GemEventData e, ReplayEventsData replayEventsData)
	{
	}
}
