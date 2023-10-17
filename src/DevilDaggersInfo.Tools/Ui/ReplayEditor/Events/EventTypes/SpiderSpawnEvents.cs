using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEventData>
{
	private static readonly string[] _spiderTypeNamesArray = EnumUtils.SpiderTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Type", ImGuiTableColumnFlags.WidthFixed, 80),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 192),
	};

	public static void Render(int eventIndex, int entityId, SpiderSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(SpiderSpawnEventData.SpiderType), ref e.SpiderType, EnumUtils.SpiderTypes, _spiderTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(SpiderSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderSpawnEventData.Position), ref e.Position, "%.2f");
	}
}
