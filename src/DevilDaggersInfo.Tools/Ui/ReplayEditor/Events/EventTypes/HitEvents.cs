using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEvent>
{
	public static void Render(int index, HitEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(HitEvent.EntityIdA), entityTypes, ref e.EntityIdA);
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(HitEvent.EntityIdB), entityTypes, ref e.EntityIdB);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(HitEvent.UserData), ref e.UserData);
	}
}
