using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class ThornSpawnEvents : IEventTypeRenderer<ThornSpawnEvent>
{
	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("?", ImGuiTableColumnFlags.None, 128),
		new("Position", ImGuiTableColumnFlags.None, 128),
		new("Rotation", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, ThornSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(ThornSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(ThornSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(ThornSpawnEvent.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}
}
