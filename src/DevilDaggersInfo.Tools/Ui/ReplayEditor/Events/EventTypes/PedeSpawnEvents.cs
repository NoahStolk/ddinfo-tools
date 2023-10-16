using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEventData>
{
	private static readonly string[] _pedeTypeNamesArray = EnumUtils.PedeTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Type", ImGuiTableColumnFlags.WidthFixed, 96),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 128),
		new("?", ImGuiTableColumnFlags.WidthFixed, 80),
		new("Orientation", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int eventIndex, int entityId, PedeSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnInputEnum(eventIndex, nameof(PedeSpawnEventData.PedeType), ref e.PedeType, EnumUtils.PedeTypes, _pedeTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(PedeSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.B), ref e.B, "%.0f");
		EventTypeRendererUtils.NextColumnInputMatrix3x3(eventIndex, nameof(PedeSpawnEventData.Orientation), ref e.Orientation, "%.2f");
	}
}
