using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DeathEvents : IEventTypeRenderer<DeathEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Death Type", ImGuiTableColumnFlags.WidthFixed, 160),
	};

	public static void Render(int index, DeathEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.NextColumnText(Inline.Span(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.DeathType)?.Name ?? "???"));
	}
}
