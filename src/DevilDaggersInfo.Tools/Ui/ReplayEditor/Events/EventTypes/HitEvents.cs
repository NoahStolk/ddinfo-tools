using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		new("Entity Id A", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Entity Id B", ImGuiTableColumnFlags.WidthFixed, 160),
		new("User Data", ImGuiTableColumnFlags.WidthFixed, 128),
	};

	public static void Render(int index, HitEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnActions(index);
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnEditableEntityId(index, nameof(HitEvent.EntityIdA), entityTypes, ref e.EntityIdA);
		EventTypeRendererUtils.NextColumnEditableEntityId(index, nameof(HitEvent.EntityIdB), entityTypes, ref e.EntityIdB);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(HitEvent.UserData), ref e.UserData);
	}
}
