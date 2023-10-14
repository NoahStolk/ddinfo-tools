using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SquidSpawnEvents : IEventTypeRenderer<SquidSpawnEvent>
{
	public static void Render(int index, SquidSpawnEvent e, IReadOnlyList<EntityType> entityTypes)
	{
		EventTypeRendererUtils.NextColumnText(Inline.Span(index));
		EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
		EventTypeRendererUtils.NextColumnText(GetSquidTypeText(e.SquidType)); // TODO: Make this a dropdown.
		EventTypeRendererUtils.NextColumnInputInt(index, nameof(SquidSpawnEvent.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SquidSpawnEvent.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SquidSpawnEvent.Direction), ref e.Direction, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(index, nameof(SquidSpawnEvent.RotationInRadians), ref e.RotationInRadians, "%.2f");
	}

	private static ReadOnlySpan<char> GetSquidTypeText(SquidType squidType) => squidType switch
	{
		SquidType.Squid1 => "Squid1",
		SquidType.Squid2 => "Squid2",
		SquidType.Squid3 => "Squid3",
		_ => throw new UnreachableException(),
	};
}
