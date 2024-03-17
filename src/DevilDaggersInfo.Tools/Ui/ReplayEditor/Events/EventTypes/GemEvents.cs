using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class GemEvents : IEventTypeRenderer<GemEventData>
{
	public static int ColumnCount => 1;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
	}

	public static void Render(int eventIndex, int entityId, GemEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(eventIndex);
	}
}
