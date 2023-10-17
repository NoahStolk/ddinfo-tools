using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class ThornSpawnEvents : IEventTypeRenderer<ThornSpawnEventData>
{
	public static int ColumnCount => 6;
	public static int ColumnCountData => 3;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, ThornSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, ThornSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(ThornSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(ThornSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(ThornSpawnEventData.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}
}
