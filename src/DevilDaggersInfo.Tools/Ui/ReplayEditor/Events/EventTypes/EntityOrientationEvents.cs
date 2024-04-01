using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEventData>
{
	public static int ColumnCount => 1;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
	}

	public static void Render(EntityOrientationEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(e.Orientation);
	}
}
