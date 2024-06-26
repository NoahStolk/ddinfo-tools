using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SquidSpawnEvents : IEventTypeRenderer<SquidSpawnEventData>
{
	public static int ColumnCount => 5;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Direction", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(SquidSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEnum(e.SquidType);
		EventTypeRendererUtils.NextColumn(e.A);
		EventTypeRendererUtils.NextColumnVector3(e.Position, "0.00");
		EventTypeRendererUtils.NextColumnVector3(e.Direction, "0.00");
		EventTypeRendererUtils.NextColumn(e.RotationInRadians, "0.00");
	}
}
