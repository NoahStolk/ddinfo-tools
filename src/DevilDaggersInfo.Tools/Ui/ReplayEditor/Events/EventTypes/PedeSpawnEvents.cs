using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEventData>
{
	private static readonly string[] _pedeTypeNamesArray = EnumUtils.PedeTypeNames.Values.ToArray();

	public static int ColumnCount => 8;
	public static int ColumnCountData => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.None, 128);
	}

	public static void Render(int eventIndex, int entityId, PedeSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, PedeSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(PedeSpawnEventData.PedeType), ref e.PedeType, EnumUtils.PedeTypes, _pedeTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(PedeSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.B), ref e.B, "%.0f");
		EventTypeRendererUtils.NextColumnInputMatrix3x3(eventIndex, nameof(PedeSpawnEventData.Orientation), ref e.Orientation, "%.2f");
	}
}
