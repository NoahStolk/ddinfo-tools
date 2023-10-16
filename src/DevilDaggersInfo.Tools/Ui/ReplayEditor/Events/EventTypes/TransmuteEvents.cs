using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEventData>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("?", ImGuiTableColumnFlags.WidthFixed, 128),
		new("?", ImGuiTableColumnFlags.WidthFixed, 128),
		new("?", ImGuiTableColumnFlags.WidthFixed, 128),
		new("?", ImGuiTableColumnFlags.WidthFixed, 128),
	};

	public static void Render(int eventIndex, int entityId, TransmuteEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(TransmuteEventData.EntityId), replayEventsData, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.C), ref e.C);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.D), ref e.D);
	}
}
