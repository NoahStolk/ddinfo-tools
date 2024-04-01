using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEventData>
{
	public static int ColumnCount => 2;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
	}

	public static void Render(EntityOrientationEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEntityId(replay, e.EntityId);
		EventTypeRendererUtils.NextColumn(e.Orientation);
	}
}
