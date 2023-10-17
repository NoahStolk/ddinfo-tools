using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEventData>
{
	public static int ColumnCount => 5;
	public static int ColumnCountData => 3;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, HitEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, HitEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdA), replayEventsData, ref e.EntityIdA);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdB), replayEventsData, ref e.EntityIdB);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(HitEventData.UserData), ref e.UserData);
	}
}
