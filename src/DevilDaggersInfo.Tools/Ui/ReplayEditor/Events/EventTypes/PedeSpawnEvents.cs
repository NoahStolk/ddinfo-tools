using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEvent>
{
	private static readonly string[] _pedeTypeNamesArray = EnumUtils.PedeTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.WidthFixed, 96),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 128),
		new("?", ImGuiTableColumnFlags.WidthFixed, 80),
		new("Orientation", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, PedeSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnEntityId(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(PedeSpawnEvent.PedeType), ref e.PedeType, EnumUtils.PedeTypes, _pedeTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(PedeSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(PedeSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(PedeSpawnEvent.B), ref e.B, "%.0f");
		EventTypeRendererUtils.NextColumnInputMatrix3x3(index, nameof(PedeSpawnEvent.Orientation), ref e.Orientation);
	}
}
