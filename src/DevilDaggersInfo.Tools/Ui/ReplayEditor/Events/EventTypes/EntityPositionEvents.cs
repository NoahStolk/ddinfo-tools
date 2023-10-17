using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityPositionEvents : IEventTypeRenderer<EntityPositionEventData>
{
	public static int ColumnCount => 4;
	public static int ColumnCountData => 2;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, EntityPositionEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, EntityPositionEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(EntityPositionEventData.EntityId), replayEventsData, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(EntityPositionEventData.Position), ref e.Position);
	}
}
