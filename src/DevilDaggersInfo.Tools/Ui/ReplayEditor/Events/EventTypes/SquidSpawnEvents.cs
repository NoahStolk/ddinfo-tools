using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SquidSpawnEvents : IEventTypeRenderer<SquidSpawnEventData>
{
	private static readonly string[] _squidTypeNamesArray = EnumUtils.SquidTypeNames.Values.ToArray();

	public static int ColumnCount => 7;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Direction", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, SquidSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(SquidSpawnEventData.SquidType), ref e.SquidType, EnumUtils.SquidTypes, _squidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(SquidSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SquidSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SquidSpawnEventData.Direction), ref e.Direction, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(SquidSpawnEventData.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}
}
