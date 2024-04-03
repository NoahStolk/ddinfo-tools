using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEventData>
{
	public static int ColumnCount => 4;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(TransmuteEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(e.A);
		EventTypeRendererUtils.NextColumn(e.B);
		EventTypeRendererUtils.NextColumn(e.C);
		EventTypeRendererUtils.NextColumn(e.D);
	}
}
