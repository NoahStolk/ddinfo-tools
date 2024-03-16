using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

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

	public static void Render(int eventIndex, int entityId, EndEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, EndEventData e, EditorReplayModel replay)
	{
	}

	public static void RenderEdit(int uniqueId, EndEventData e, EditorReplayModel replay)
	{
	}
}
