using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class GemEvents : IEventTypeRenderer<GemEventData>
{
	public static int ColumnCount => 0;

	public static void SetupColumns()
	{
	}

	public static void Render(GemEventData e, EditorReplayModel replay)
	{
	}
}
