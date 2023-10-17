using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SquidSpawnEvents : IEventTypeRenderer<SquidSpawnEventData>
{
	private static readonly string[] _squidTypeNamesArray = EnumUtils.SquidTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		EventColumn.Actions,
		EventColumn.Index,
		EventColumn.EntityId,
		new("Type", ImGuiTableColumnFlags.WidthFixed, 80),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.WidthFixed, 192),
		new("Direction", ImGuiTableColumnFlags.WidthFixed, 192),
		new("Rotation", ImGuiTableColumnFlags.WidthFixed, 64),
	};

	public static void Render(int eventIndex, int entityId, SquidSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(SquidSpawnEventData.SquidType), ref e.SquidType, EnumUtils.SquidTypes, _squidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(SquidSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SquidSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SquidSpawnEventData.Direction), ref e.Direction, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(SquidSpawnEventData.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}
}
