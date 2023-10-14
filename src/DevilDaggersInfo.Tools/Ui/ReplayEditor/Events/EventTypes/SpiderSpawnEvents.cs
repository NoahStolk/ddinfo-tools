using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEvent>
{
	public static void Render(int index, SpiderSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnText(GetSpiderTypeText(e.SpiderType)); // TODO: Make this a dropdown.
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderSpawnEvent.Position), ref e.Position, "%.2f");
	}

	private static ReadOnlySpan<char> GetSpiderTypeText(SpiderType spiderType) => spiderType switch
	{
		SpiderType.Spider1 => "Spider1",
		SpiderType.Spider2 => "Spider2",
		_ => throw new UnreachableException(),
	};
}
