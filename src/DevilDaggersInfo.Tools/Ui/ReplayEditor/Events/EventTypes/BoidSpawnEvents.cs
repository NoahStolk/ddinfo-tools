using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEventData>
{
	public static int ColumnCount => 8;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
		ImGui.TableSetupColumn("Velocity", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Speed", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, BoidSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		EventTypeRendererUtils.NextColumnEntityId(replay, e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnEnum(e.BoidType);
		EventTypeRendererUtils.NextColumn(e.Position);
		EventTypeRendererUtils.NextColumn(e.Orientation);
		EventTypeRendererUtils.NextColumnVector3(e.Velocity, "0.00");
		EventTypeRendererUtils.NextColumn(e.Speed, "0.00");
	}
}
