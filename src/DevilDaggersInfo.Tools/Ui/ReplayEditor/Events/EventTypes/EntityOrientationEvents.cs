using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEventData>
{
	public static int ColumnCount => 3;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
	}

	public static void Render(int eventIndex, int entityId, EntityOrientationEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, e.EntityId);
		EventTypeRendererUtils.NextColumn(e.Orientation);
	}
}
