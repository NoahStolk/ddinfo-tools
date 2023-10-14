using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEvent>
{
	public static void Render(int index, PedeSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnText(GetPedeTypeText(e.PedeType)); // TODO: Make this a dropdown.
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(PedeSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(PedeSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(PedeSpawnEvent.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputMatrix3x3(index, nameof(PedeSpawnEvent.Orientation), ref e.Orientation);
	}

	private static ReadOnlySpan<char> GetPedeTypeText(PedeType pedeType) => pedeType switch
	{
		PedeType.Centipede => "Centipede",
		PedeType.Gigapede => "Gigapede",
		PedeType.Ghostpede => "Ghostpede",
		_ => throw new UnreachableException(),
	};
}
