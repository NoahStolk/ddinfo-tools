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

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Type", ImGuiTableColumnFlags.WidthFixed, 96),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Orientation", ImGuiTableColumnFlags.WidthFixed, 384),
		new("Shot/Rapid", ImGuiTableColumnFlags.WidthFixed, 80),
	};

	public static void Render(int eventIndex, int entityId, DaggerSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnInputEnum(eventIndex, nameof(DaggerSpawnEventData.DaggerType), ref e.DaggerType, EnumUtils.DaggerTypes, _daggerTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(DaggerSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(DaggerSpawnEventData.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(DaggerSpawnEventData.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnCheckbox(eventIndex, nameof(DaggerSpawnEventData.IsShot), ref e.IsShot, "Shot", "Rapid");
	}
}
