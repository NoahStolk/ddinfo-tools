using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEventData>
{
	public static int ColumnCount => 3;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 192);
	}

	public static void Render(SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEntityId(replay, e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnVector3(e.Position, "0.00");
		EventTypeRendererUtils.NextColumnVector3(e.TargetPosition, "0.00");
	}
}
