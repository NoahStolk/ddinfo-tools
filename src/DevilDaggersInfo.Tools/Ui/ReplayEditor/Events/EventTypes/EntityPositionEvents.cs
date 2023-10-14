using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityPositionEvents : IEventTypeRenderer<EntityPositionEvent>
{
	public static void Render(int index, EntityPositionEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(EntityPositionEvent.EntityId), entityTypes, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(EntityPositionEvent.Position), ref e.Position);
	}
}
