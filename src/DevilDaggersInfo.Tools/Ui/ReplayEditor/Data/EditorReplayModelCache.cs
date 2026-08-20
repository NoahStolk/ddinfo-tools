using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

internal sealed record EditorReplayModelCache(IReadOnlyList<ReplayEvent> Events, IReadOnlyList<EntityType> Entities, IReadOnlyDictionary<int, int> EntityIdByEventIndex);
