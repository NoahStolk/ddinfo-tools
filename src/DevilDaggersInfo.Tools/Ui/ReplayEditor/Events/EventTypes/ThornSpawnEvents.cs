using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class ThornSpawnEvents : IEventTypeRenderer<ThornSpawnEvent>
{
	public static void Render(int index, ThornSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(ThornSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(ThornSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(ThornSpawnEvent.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}
}
