using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Orientation", ImGuiTableColumnFlags.None, 196),
	};

	public static void Render(int index, EntityOrientationEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(EntityOrientationEvent.EntityId), entityTypes, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(EntityOrientationEvent.Orientation), ref e.Orientation);
	}
}
