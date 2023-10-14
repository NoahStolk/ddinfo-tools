using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEvent>
{
	private static readonly string[] _spiderTypeNamesArray = EnumUtils.SpiderTypeNames.Values.ToArray();

	public static void Render(int index, SpiderSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(SpiderSpawnEvent.SpiderType), ref e.SpiderType, EnumUtils.SpiderTypes, _spiderTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderSpawnEvent.Position), ref e.Position, "%.2f");
	}
}
