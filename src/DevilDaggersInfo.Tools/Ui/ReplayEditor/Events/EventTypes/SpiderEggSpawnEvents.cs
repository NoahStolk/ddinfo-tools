using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Spawner Entity Id", ImGuiTableColumnFlags.None, 196),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Target Position", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, SpiderEggSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderEggSpawnEvent.SpawnerEntityId), ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderEggSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderEggSpawnEvent.TargetPosition), ref e.TargetPosition, "%.2f");
	}
}
