using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEventData>
{
	public static int ColumnCount => 1;

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(EntityTargetEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumn(e.TargetPosition);
	}
}
