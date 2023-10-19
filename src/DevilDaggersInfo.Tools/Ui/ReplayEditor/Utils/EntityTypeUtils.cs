using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;

public static class EntityTypeUtils
{
	/// <summary>
	/// Pede entity IDs may be negated.
	/// TODO: This could be moved to DevilDaggersInfo.Core.Replay with proper unit tests.
	/// </summary>
	public static EntityType? GetEntityTypeIncludingNegated(ReplayEventsData replayEventsData, int entityId)
	{
		int absoluteEntityId = Math.Abs(entityId);
		if (absoluteEntityId > replayEventsData.SpawnEventCount)
			return null;

		EntityType entityType = replayEventsData.GetEntityType(absoluteEntityId);
		if (entityId < 0 && entityType is not EntityType.Centipede and not EntityType.Gigapede and not EntityType.Ghostpede)
			return null;

		return entityType;
	}
}
