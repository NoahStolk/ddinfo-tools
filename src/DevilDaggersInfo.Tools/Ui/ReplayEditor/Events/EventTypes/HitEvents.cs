using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEventData>
{
	public static int ColumnCount => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		ImGui.TableSetupColumn("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Explanation", ImGuiTableColumnFlags.WidthStretch);
	}

	public static void Render(int eventIndex, int entityId, HitEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdA), replay, ref e.EntityIdA);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdB), replay, ref e.EntityIdB);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(HitEventData.UserData), ref e.UserData);

		ImGui.TableNextColumn();
		HitEventExplanation.Render(e, replay);
	}
}
