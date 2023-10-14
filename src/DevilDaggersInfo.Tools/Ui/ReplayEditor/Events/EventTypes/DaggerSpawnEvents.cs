using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DaggerSpawnEvents : IEventTypeRenderer<DaggerSpawnEvent>
{
	public static void Render(int index, DaggerSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnText(EnumUtils.DaggerTypeNames[e.DaggerType]); // TODO: Make this a dropdown.
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(DaggerSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(DaggerSpawnEvent.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(DaggerSpawnEvent.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnCheckbox(index, nameof(DaggerSpawnEvent.IsShot), ref e.IsShot, "Shot", "Rapid");
	}
}
