using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEvent>
{
	public static void Render(int index, EntityOrientationEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(EntityOrientationEvent.Orientation), ref e.Orientation);
	}
}
