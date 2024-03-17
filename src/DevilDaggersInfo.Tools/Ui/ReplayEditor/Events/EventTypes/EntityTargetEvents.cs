using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEventData>
{
	public static int ColumnCount => 3;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, EntityTargetEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(EntityTargetEventData.TargetPosition), ref e.TargetPosition);
	}
}
