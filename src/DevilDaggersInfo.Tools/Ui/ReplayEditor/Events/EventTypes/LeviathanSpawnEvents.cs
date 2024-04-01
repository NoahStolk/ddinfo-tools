using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class LeviathanSpawnEvents : IEventTypeRenderer<LeviathanSpawnEventData>
{
	public static int ColumnCount => 1;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
	}

	public static void Render(LeviathanSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(e.A);
	}
}
