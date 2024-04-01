using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEventData>
{
	public static int ColumnCount => 5;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.None, 128);
	}

	public static void Render(PedeSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEnum(e.PedeType);
		EventTypeRendererUtils.NextColumn(e.A);
		EventTypeRendererUtils.NextColumnVector3(e.Position, "0.00");
		EventTypeRendererUtils.NextColumnVector3(e.B, "0");
		EventTypeRendererUtils.NextColumn(e.Orientation, "0.00");
	}
}
