using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEvent>
{
	private static readonly string[] _boidTypeNamesArray = EnumUtils.BoidTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 96),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
		new("Velocity", ImGuiTableColumnFlags.WidthFixed, 128),
		new("Speed", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	public static void Render(int index, BoidSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnEntityId(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnEditableEntityId(index, nameof(BoidSpawnEvent.SpawnerEntityId), entityTypes, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(BoidSpawnEvent.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(BoidSpawnEvent.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(BoidSpawnEvent.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(BoidSpawnEvent.Velocity), ref e.Velocity, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(BoidSpawnEvent.Speed), ref e.Speed, "%.2f");
	}
}
