using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEventData>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		new("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160),
		new("User Data", ImGuiTableColumnFlags.WidthFixed, 128),
	};

	public static void Render(int eventIndex, int entityId, HitEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdA), replayEventsData, ref e.EntityIdA);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(HitEventData.EntityIdB), replayEventsData, ref e.EntityIdB);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(HitEventData.UserData), ref e.UserData);
	}
}
