using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DaggerSpawnEvents : IEventTypeRenderer<DaggerSpawnEvent>
{
	private static readonly string[] _daggerTypeNamesArray = EnumUtils.DaggerTypeNames.Values.ToArray();

	public static void Render(int index, DaggerSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnInputEnum(index, nameof(DaggerSpawnEvent.DaggerType), ref e.DaggerType, EnumUtils.DaggerTypes, _daggerTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(DaggerSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(DaggerSpawnEvent.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(DaggerSpawnEvent.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(DaggerSpawnEvent.IsShot), ref e.IsShot, "Shot", "Rapid");
	}
}
