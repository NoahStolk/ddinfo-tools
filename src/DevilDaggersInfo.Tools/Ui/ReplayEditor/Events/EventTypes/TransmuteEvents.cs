using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEventData>
{
	public static int ColumnCount => 6;
	public static int ColumnCountData => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, TransmuteEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, TransmuteEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(TransmuteEventData.EntityId), replay, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.C), ref e.C);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.D), ref e.D);
	}
}
