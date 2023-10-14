using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class LeviathanSpawnEvents : IEventTypeRenderer<LeviathanSpawnEvent>
{
	public static void Render(int index, LeviathanSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(LeviathanSpawnEvent.A), ref e.A);
	}
}
