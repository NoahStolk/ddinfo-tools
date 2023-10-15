using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Target Position", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, EntityTargetEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnEditableEntityId(index, nameof(EntityTargetEvent.EntityId), entityTypes, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(EntityTargetEvent.TargetPosition), ref e.TargetPosition);
	}
}
