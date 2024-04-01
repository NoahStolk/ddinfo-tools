using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEventData>
{
	public static int ColumnCount => 4;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("User Data", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Explanation", ImGuiTableColumnFlags.WidthStretch);
	}

	public static void Render(int entityId, HitEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEntityId(replay, e.EntityIdA);
		EventTypeRendererUtils.NextColumnEntityId(replay, e.EntityIdB);
		EventTypeRendererUtils.NextColumn(e.UserData);

		ImGui.TableNextColumn();
		HitEventExplanation.Render(e, replay);
	}
}
