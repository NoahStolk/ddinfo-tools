using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Utils;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class TimelineCache
{
	private static readonly Dictionary<EventType, Dictionary<int, int>> _eventCountsPerTick = EnumUtils.EventTypes.Where(e => e is not EventType.End and not EventType.InitialInputs and not EventType.Inputs).ToDictionary(eventType => eventType, _ => new Dictionary<int, int>());

	public static IReadOnlyDictionary<EventType, Dictionary<int, int>> EventCountsPerTick => _eventCountsPerTick;

	public static bool IsEmpty { get; private set; }

	public static void Clear()
	{
		foreach (Dictionary<int, int> eventCounts in _eventCountsPerTick.Values)
			eventCounts.Clear();

		IsEmpty = true;
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
				if (!_eventCountsPerTick[eventType].TryGetValue(editorEvent.TickIndex, out int boidEventCount))
					_eventCountsPerTick[eventType][editorEvent.TickIndex] = 0;
				_eventCountsPerTick[eventType][editorEvent.TickIndex] = ++boidEventCount;
			}
		}
	}
}
