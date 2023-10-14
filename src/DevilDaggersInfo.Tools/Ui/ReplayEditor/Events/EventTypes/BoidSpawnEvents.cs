using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEvent>
{
	public static void Render(int index, BoidSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.EntityColumn(entityTypes, e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnText(GetBoidTypeText(e.BoidType)); // TODO: Make this a dropdown.
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(BoidSpawnEvent.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(BoidSpawnEvent.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(BoidSpawnEvent.Velocity), ref e.Velocity, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(BoidSpawnEvent.Speed), ref e.Speed, "%.2f");
	}

	private static ReadOnlySpan<char> GetBoidTypeText(BoidType boidType) => boidType switch
	{
		BoidType.Skull1 => "Skull1",
		BoidType.Skull2 => "Skull2",
		BoidType.Skull3 => "Skull3",
		BoidType.Skull4 => "Skull4",
		BoidType.Spiderling => "Spiderling",
		_ => throw new UnreachableException(),
	};
}
