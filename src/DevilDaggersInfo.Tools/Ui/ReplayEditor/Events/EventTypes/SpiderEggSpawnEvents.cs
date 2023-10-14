using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEvent>
{
	public static void Render(int index, SpiderEggSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderEggSpawnEvent.SpawnerEntityId), ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderEggSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderEggSpawnEvent.TargetPosition), ref e.TargetPosition, "%.2f");
	}
}
