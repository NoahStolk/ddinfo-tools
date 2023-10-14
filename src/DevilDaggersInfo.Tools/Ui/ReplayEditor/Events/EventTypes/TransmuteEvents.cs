using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("?", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, TransmuteEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EditableEntityColumn(index, nameof(TransmuteEvent.EntityId), entityTypes, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.C), ref e.C);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(TransmuteEvent.D), ref e.D);
	}
}
