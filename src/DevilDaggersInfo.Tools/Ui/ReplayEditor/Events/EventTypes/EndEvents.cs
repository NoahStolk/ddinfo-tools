using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EndEvents : IEventTypeRenderer<EndEventData>
{
	public static int ColumnCount => 0;

	public static void SetupColumns()
	{
	}

	public static void Render(EndEventData e, EditorReplayModel replay)
	{
	}
}
