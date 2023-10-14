using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEvent>
{
	public static void Render(int index, EntityTargetEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(EntityTargetEvent.EntityId), entityTypes, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(EntityTargetEvent.TargetPosition), ref e.TargetPosition);
	}
}
