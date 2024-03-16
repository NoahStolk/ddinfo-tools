using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EndEvents : IEventTypeRenderer<EndEventData>
{
	public static int ColumnCount => 1;
	public static int ColumnCountData => 0;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
	}

	public static void Render(int eventIndex, int entityId, EndEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, EndEventData e, ReplayEventsData replayEventsData)
	{
	}

	public static void RenderEdit(int eventIndex, EndEventData e, ReplayEventsData replayEventsData)
	{
	}
}
