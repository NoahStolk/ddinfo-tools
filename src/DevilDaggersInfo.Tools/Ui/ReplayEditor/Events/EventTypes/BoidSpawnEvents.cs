using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEventData>
{
	private static readonly string[] _boidTypeNamesArray = EnumUtils.BoidTypeNames.Values.ToArray();

	public static int ColumnCount => 8;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.None, 196);
		ImGui.TableSetupColumn("Velocity", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Speed", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, BoidSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		EventTypeRendererUtils.NextColumnEntityId(replay, e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(BoidSpawnEventData.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(BoidSpawnEventData.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(BoidSpawnEventData.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(BoidSpawnEventData.Velocity), ref e.Velocity, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(BoidSpawnEventData.Speed), ref e.Speed, "%.2f");
	}
}
