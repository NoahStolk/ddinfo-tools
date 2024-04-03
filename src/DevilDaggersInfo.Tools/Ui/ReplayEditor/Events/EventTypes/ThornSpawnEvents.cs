using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class ThornSpawnEvents : IEventTypeRenderer<ThornSpawnEventData>
{
	public static int ColumnCount => 3;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Rotation", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(ThornSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(e.A);
		EventTypeRendererUtils.NextColumnVector3(e.Position, "0.00");
		EventTypeRendererUtils.NextColumn(e.RotationInRadians, "0.00");
	}
}
