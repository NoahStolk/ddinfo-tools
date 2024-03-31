using DevilDaggersInfo.Core.Replay.Events;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public sealed record EditorReplayModelCache
{
	public EditorReplayModelCache(IReadOnlyList<ReplayEvent> replayEvents, IReadOnlyList<EntitySpawnReplayEvent> entitySpawnReplayEvents)
	{
		Events = replayEvents;
		EntitySpawnReplayEvents = entitySpawnReplayEvents;
	}

	public IReadOnlyList<ReplayEvent> Events { get; }
	public IReadOnlyList<EntitySpawnReplayEvent> EntitySpawnReplayEvents { get; }
}
