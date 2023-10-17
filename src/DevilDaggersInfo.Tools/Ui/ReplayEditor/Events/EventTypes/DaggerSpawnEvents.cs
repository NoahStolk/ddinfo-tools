using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DaggerSpawnEvents : IEventTypeRenderer<DaggerSpawnEventData>
{
	private static readonly string[] _daggerTypeNamesArray =
	{
		"Lvl1",
		"Lvl2",
		"Lvl3",
		"Lvl3 Homing",
		"Lvl4",
		"Lvl4 Homing",
		"Lvl4 Splash",
	};

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
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
		ImGui.TableSetupColumn("Shot/Rapid", ImGuiTableColumnFlags.WidthFixed, 80);
	}

	public static void Render(int eventIndex, int entityId, DaggerSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, DaggerSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(DaggerSpawnEventData.DaggerType), ref e.DaggerType, EnumUtils.DaggerTypes, _daggerTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(DaggerSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(DaggerSpawnEventData.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(DaggerSpawnEventData.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(DaggerSpawnEventData.IsShot), ref e.IsShot, "Shot", "Rapid");
	}
}
