using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class TimelineCache
{
	private static readonly Dictionary<int, TimelineCacheEntry> _cache = new();

	public static bool IsEmpty { get; private set; }

	public static void Clear()
	{
		_cache.Clear();

		IsEmpty = true;
	}

	public static int GetEventCountAtTick(int tickIndex, EventType eventType)
	{
		if (!_cache.TryGetValue(tickIndex, out TimelineCacheEntry? cacheEntry))
			return 0;

		switch (eventType)
		{
			case EventType.BoidSpawn: return cacheEntry.BoidSpawnEventCount;
			case EventType.DaggerSpawn: return cacheEntry.DaggerSpawnEventCount;
			case EventType.EntityOrientation: return cacheEntry.EntityOrientationEventCount;
			case EventType.EntityPosition: return cacheEntry.EntityPositionEventCount;
			case EventType.EntityTarget: return cacheEntry.EntityTargetEventCount;
			case EventType.Gem: return cacheEntry.GemEventCount;
			case EventType.Hit: return cacheEntry.HitEventCount;
			case EventType.LeviathanSpawn: return cacheEntry.LeviathanSpawnEventCount;
			case EventType.PedeSpawn: return cacheEntry.PedeSpawnEventCount;
			case EventType.SpiderEggSpawn: return cacheEntry.SpiderEggSpawnEventCount;
			case EventType.SpiderSpawn: return cacheEntry.SpiderSpawnEventCount;
			case EventType.SquidSpawn: return cacheEntry.SquidSpawnEventCount;
			case EventType.ThornSpawn: return cacheEntry.ThornSpawnEventCount;
			case EventType.Transmute: return cacheEntry.TransmuteEventCount;
			case EventType.InitialInputs:
			case EventType.Inputs:
			case EventType.End:
			default: throw new UnreachableException($"Unexpected event type {eventType}.");
		}
	}

	public static void Build(EditorReplayModel replayModel)
	{
		Clear();

		AddCounts(replayModel.BoidSpawnEvents, EventType.BoidSpawn);
		AddCounts(replayModel.DaggerSpawnEvents, EventType.DaggerSpawn);
		AddCounts(replayModel.EntityOrientationEvents, EventType.EntityOrientation);
		AddCounts(replayModel.EntityPositionEvents, EventType.EntityPosition);
		AddCounts(replayModel.EntityTargetEvents, EventType.EntityTarget);
		AddCounts(replayModel.GemEvents, EventType.Gem);
		AddCounts(replayModel.HitEvents, EventType.Hit);
		AddCounts(replayModel.LeviathanSpawnEvents, EventType.LeviathanSpawn);
		AddCounts(replayModel.PedeSpawnEvents, EventType.PedeSpawn);
		AddCounts(replayModel.SpiderEggSpawnEvents, EventType.SpiderEggSpawn);
		AddCounts(replayModel.SpiderSpawnEvents, EventType.SpiderSpawn);
		AddCounts(replayModel.SquidSpawnEvents, EventType.SquidSpawn);
		AddCounts(replayModel.ThornSpawnEvents, EventType.ThornSpawn);
		AddCounts(replayModel.TransmuteEvents, EventType.Transmute);

		IsEmpty = false;

		void AddCounts(IReadOnlyList<EditorEvent> events, EventType eventType)
		{
			for (int i = 0; i < events.Count; i++)
			{
				EditorEvent editorEvent = events[i];

				if (!_cache.TryGetValue(editorEvent.TickIndex, out TimelineCacheEntry? cacheEntry))
				{
					cacheEntry = new TimelineCacheEntry();
					_cache.Add(editorEvent.TickIndex, cacheEntry);
				}

				switch (eventType)
				{
					case EventType.BoidSpawn: cacheEntry.BoidSpawnEventCount++; break;
					case EventType.DaggerSpawn: cacheEntry.DaggerSpawnEventCount++; break;
					case EventType.EntityOrientation: cacheEntry.EntityOrientationEventCount++; break;
					case EventType.EntityPosition: cacheEntry.EntityPositionEventCount++; break;
					case EventType.EntityTarget: cacheEntry.EntityTargetEventCount++; break;
					case EventType.Gem: cacheEntry.GemEventCount++; break;
					case EventType.Hit: cacheEntry.HitEventCount++; break;
					case EventType.LeviathanSpawn: cacheEntry.LeviathanSpawnEventCount++; break;
					case EventType.PedeSpawn: cacheEntry.PedeSpawnEventCount++; break;
					case EventType.SpiderEggSpawn: cacheEntry.SpiderEggSpawnEventCount++; break;
					case EventType.SpiderSpawn: cacheEntry.SpiderSpawnEventCount++; break;
					case EventType.SquidSpawn: cacheEntry.SquidSpawnEventCount++; break;
					case EventType.ThornSpawn: cacheEntry.ThornSpawnEventCount++; break;
					case EventType.Transmute: cacheEntry.TransmuteEventCount++; break;
					case EventType.InitialInputs:
					case EventType.Inputs:
					case EventType.End:
					default: throw new UnreachableException($"Unexpected event type {eventType}.");
				}
			}
		}
	}
}
