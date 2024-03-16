using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class GemEvents : IEventTypeRenderer<GemEventData>
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

	public static void Render(int eventIndex, int entityId, GemEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, GemEventData e, EditorReplayModel replay)
	{
	}

	public static void RenderEdit(int uniqueId, GemEventData e, EditorReplayModel replay)
	{
	}
}
