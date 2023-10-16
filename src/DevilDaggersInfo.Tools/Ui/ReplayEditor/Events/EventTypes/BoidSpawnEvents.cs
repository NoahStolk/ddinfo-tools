using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEventData>
{
	private static readonly string[] _boidTypeNamesArray = EnumUtils.BoidTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
		new("Velocity", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Speed", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	public static void Render(int eventIndex, int entityId, BoidSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(BoidSpawnEventData.SpawnerEntityId), replayEventsData, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputEnum(eventIndex, nameof(BoidSpawnEventData.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(BoidSpawnEventData.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(BoidSpawnEventData.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(BoidSpawnEventData.Velocity), ref e.Velocity, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(BoidSpawnEventData.Speed), ref e.Speed, "%.2f");
	}
}
