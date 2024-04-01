using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public sealed record EditorReplayModelCache
{
	public EditorReplayModelCache(IReadOnlyList<ReplayEvent> replayEvents, IReadOnlyList<EntityType> entities)
	{
		Events = replayEvents;
		Entities = entities;
	}

	public IReadOnlyList<ReplayEvent> Events { get; }
	public IReadOnlyList<EntityType> Entities { get; }
}
