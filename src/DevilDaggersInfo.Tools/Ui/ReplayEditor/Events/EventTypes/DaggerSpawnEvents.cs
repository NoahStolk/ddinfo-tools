using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DaggerSpawnEvents : IEventTypeRenderer<DaggerSpawnEventData>
{
	public static int ColumnCount => 6;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
		ImGui.TableSetupColumn("Shot/Rapid", ImGuiTableColumnFlags.WidthFixed, 80);
	}

	public static void Render(int entityId, DaggerSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		EventTypeRendererUtils.NextColumnEnum(e.DaggerType);
		EventTypeRendererUtils.NextColumn(e.A);
		EventTypeRendererUtils.NextColumn(e.Position);
		EventTypeRendererUtils.NextColumn(e.Orientation);
		EventTypeRendererUtils.NextColumnBool(e.IsShot, "Shot", "Rapid");
	}
}
