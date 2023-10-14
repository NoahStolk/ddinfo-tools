using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEvent>
{
	public static void Render(int index, TransmuteEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.C), ref e.C);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.D), ref e.D);
	}
}
