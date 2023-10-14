using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DeathEvents : IEventTypeRenderer<DeathEvent>
{
	public static void Render(int index, DeathEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.NextColumnText(Inline.Span(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.DeathType)?.Name ?? "???"));
	}
}
