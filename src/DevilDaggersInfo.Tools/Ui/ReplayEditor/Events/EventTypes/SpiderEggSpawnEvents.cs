using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEventData>
{
	public static int ColumnCount => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 192);
	}

	public static void Render(int eventIndex, int entityId, SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(SpiderEggSpawnEventData.SpawnerEntityId), replay, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.TargetPosition), ref e.TargetPosition, "%.2f");
	}
}
