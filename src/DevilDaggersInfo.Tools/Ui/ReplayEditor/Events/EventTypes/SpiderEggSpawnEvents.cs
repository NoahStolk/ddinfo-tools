using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEventData>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 192),
		new("Target Position", ImGuiTableColumnFlags.WidthFixed, 192),
	};

	public static void Render(int eventIndex, int entityId, SpiderEggSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(SpiderEggSpawnEventData.SpawnerEntityId), replayEventsData, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.TargetPosition), ref e.TargetPosition, "%.2f");
	}
}
