using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DeathEvents : IEventTypeRenderer<DeathEventData>
{
	public static int ColumnCount => 3;
	public static int ColumnCountData => 1;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Death Type", ImGuiTableColumnFlags.WidthFixed, 160);
	}

	public static void Render(int eventIndex, int entityId, DeathEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, DeathEventData e, ReplayEventsData replayEventsData)
	{
		ImGui.TableNextColumn();
		ImGui.Text(Inline.Span(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.DeathType)?.Name ?? "???"));
	}
}
