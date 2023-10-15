using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEvent>
{
	private static readonly string[] _spiderTypeNamesArray = EnumUtils.SpiderTypeNames.Values.ToArray();

	public static IReadOnlyList<EventColumn> EventColumns { get; } = new List<EventColumn>
	{
		new("Index", ImGuiTableColumnFlags.WidthFixed, 64),
		new("Entity Id", ImGuiTableColumnFlags.WidthFixed, 160),
		new("Type", ImGuiTableColumnFlags.WidthFixed, 80),
		new("?", ImGuiTableColumnFlags.WidthFixed, 32),
		new("Position", ImGuiTableColumnFlags.None, 128),
	};

	public static void Render(int index, SpiderSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnEventIndex(index);
		EventTypeRendererUtils.NextColumnEntityId(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(SpiderSpawnEvent.SpiderType), ref e.SpiderType, EnumUtils.SpiderTypes, _spiderTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderSpawnEvent.Position), ref e.Position, "%.2f");
	}
}
