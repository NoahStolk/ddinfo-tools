using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public record EditorReplayModel
{
	private EditorReplayModelCache? _cache;

	private readonly List<EditorEvent> _boidSpawnEvents = [];
	private readonly List<EditorEvent> _daggerSpawnEvents = [];
	private readonly List<EditorEvent> _entityOrientationEvents = [];
	private readonly List<EditorEvent> _entityPositionEvents = [];
	private readonly List<EditorEvent> _entityTargetEvents = [];
	private readonly List<EditorEvent> _gemEvents = [];
	private readonly List<EditorEvent> _hitEvents = [];
	private readonly List<EditorEvent> _leviathanSpawnEvents = [];
	private readonly List<EditorEvent> _pedeSpawnEvents = [];
	private readonly List<EditorEvent> _spiderEggSpawnEvents = [];
	private readonly List<EditorEvent> _spiderSpawnEvents = [];
	private readonly List<EditorEvent> _squidSpawnEvents = [];
	private readonly List<EditorEvent> _thornSpawnEvents = [];
	private readonly List<EditorEvent> _transmuteEvents = [];

	private EditorReplayModel(int version, long timestampSinceGameRelease, float time, float startTime, int daggersFired, int deathType, int gems, int daggersHit, int kills, int playerId, string username, SpawnsetBinary spawnset)
	{
		Version = version;
		TimestampSinceGameRelease = timestampSinceGameRelease;
		Time = time;
		StartTime = startTime;
		DaggersFired = daggersFired;
		DeathType = deathType;
		Gems = gems;
		DaggersHit = daggersHit;
		Kills = kills;
		PlayerId = playerId;
		Username = username;
		Spawnset = spawnset;
	}

	// Data found in local replay header.
	public int Version { get; set; }
	public long TimestampSinceGameRelease { get; set; }
	public float Time { get; set; }
	public float StartTime { get; set; }
	public int DaggersFired { get; set; }
	public int DeathType { get; set; }
	public int Gems { get; set; }
	public int DaggersHit { get; set; }
	public int Kills { get; set; }
	public int PlayerId { get; set; }
	public string Username { get; set; }
	public SpawnsetBinary Spawnset { get; set; }

	// TODO: Invalidate events data cache when any of the below properties are changed or values are added/removed/moved from lists.

	// Embedded inputs data.
	public float LookSpeed { get; set; }

	// TODO: Use IReadOnlyList<> instead of List<>.
	public List<InputsEventData> InputsEvents { get; } = [];

	// All other events.
	public IReadOnlyList<EditorEvent> BoidSpawnEvents => _boidSpawnEvents;
	public IReadOnlyList<EditorEvent> DaggerSpawnEvents => _daggerSpawnEvents;
	public IReadOnlyList<EditorEvent> EntityOrientationEvents => _entityOrientationEvents;
	public IReadOnlyList<EditorEvent> EntityPositionEvents => _entityPositionEvents;
	public IReadOnlyList<EditorEvent> EntityTargetEvents => _entityTargetEvents;
	public IReadOnlyList<EditorEvent> GemEvents => _gemEvents;
	public IReadOnlyList<EditorEvent> HitEvents => _hitEvents;
	public IReadOnlyList<EditorEvent> LeviathanSpawnEvents => _leviathanSpawnEvents;
	public IReadOnlyList<EditorEvent> PedeSpawnEvents => _pedeSpawnEvents;
	public IReadOnlyList<EditorEvent> SpiderEggSpawnEvents => _spiderEggSpawnEvents;
	public IReadOnlyList<EditorEvent> SpiderSpawnEvents => _spiderSpawnEvents;
	public IReadOnlyList<EditorEvent> SquidSpawnEvents => _squidSpawnEvents;
	public IReadOnlyList<EditorEvent> ThornSpawnEvents => _thornSpawnEvents;
	public IReadOnlyList<EditorEvent> TransmuteEvents => _transmuteEvents;

	public int TickCount => InputsEvents.Count;

	public EditorReplayModelCache Cache => _cache ??= RebuildCache();

	#region Factory methods

	public static EditorReplayModel CreateDefault()
	{
		return new EditorReplayModel(
			version: 1,
			timestampSinceGameRelease: 0,
			time: 0,
			startTime: 0,
			daggersFired: 0,
			deathType: 0,
			gems: 0,
			daggersHit: 0,
			kills: 0,
			playerId: 0,
			username: string.Empty,
			spawnset: SpawnsetBinary.CreateDefault());
	}

	public static EditorReplayModel CreateFromLeaderboardReplay(int playerId, string username, IReadOnlyList<ReplayEvent> replayEvents)
	{
		EditorReplayModel replay = CreateDefault();
		replay.PlayerId = playerId;
		replay.Username = username;
		replay.AddReplayEventsData(replayEvents);
		return replay;
	}

	public static EditorReplayModel CreateFromLocalReplay(ReplayBinary<LocalReplayBinaryHeader> localReplay)
	{
		EditorReplayModel replay = new(
			version: localReplay.Header.Version,
			timestampSinceGameRelease: localReplay.Header.TimestampSinceGameRelease,
			time: localReplay.Header.Time,
			startTime: localReplay.Header.StartTime,
			daggersFired: localReplay.Header.DaggersFired,
			deathType: localReplay.Header.DeathType,
			gems: localReplay.Header.Gems,
			daggersHit: localReplay.Header.DaggersHit,
			kills: localReplay.Header.Kills,
			playerId: localReplay.Header.PlayerId,
			username: localReplay.Header.Username,
			spawnset: localReplay.Header.Spawnset);
		replay.AddReplayEventsData(localReplay.Events);
		return replay;
	}

	private void AddReplayEventsData(IReadOnlyList<ReplayEvent> replayEvents)
	{
		int currentTick = 0;
		int entityId = 1;
		foreach (IEventData eventData in replayEvents.Select(e => e.Data))
		{
			switch (eventData)
			{
				case InputsEventData inputsEventData:
					InputsEvents.Add(inputsEventData);
					currentTick++;
					break;
				case InitialInputsEventData initialInputsEventData:
					InputsEvents.Add(new InputsEventData(
						Left: initialInputsEventData.Left,
						Right: initialInputsEventData.Right,
						Forward: initialInputsEventData.Forward,
						Backward: initialInputsEventData.Backward,
						Jump: initialInputsEventData.Jump,
						Shoot: initialInputsEventData.Shoot,
						ShootHoming: initialInputsEventData.ShootHoming,
						MouseX: initialInputsEventData.MouseX,
						MouseY: initialInputsEventData.MouseY));
					LookSpeed = initialInputsEventData.LookSpeed;
					currentTick++;
					break;
				case BoidSpawnEventData boidSpawnEventData: _boidSpawnEvents.Add(new EditorEvent(currentTick, entityId++, boidSpawnEventData)); break;
				case DaggerSpawnEventData daggerSpawnEventData: _daggerSpawnEvents.Add(new EditorEvent(currentTick, entityId++, daggerSpawnEventData)); break;
				case EntityOrientationEventData entityOrientationEventData: _entityOrientationEvents.Add(new EditorEvent(currentTick, null, entityOrientationEventData)); break;
				case EntityPositionEventData entityPositionEventData: _entityPositionEvents.Add(new EditorEvent(currentTick, null, entityPositionEventData)); break;
				case EntityTargetEventData entityTargetEventData: _entityTargetEvents.Add(new EditorEvent(currentTick, null, entityTargetEventData)); break;
				case GemEventData gemEventData: _gemEvents.Add(new EditorEvent(currentTick, null, gemEventData)); break;
				case HitEventData hitEventData: _hitEvents.Add(new EditorEvent(currentTick, null, hitEventData)); break;
				case LeviathanSpawnEventData leviathanSpawnEventData: _leviathanSpawnEvents.Add(new EditorEvent(currentTick, entityId++, leviathanSpawnEventData)); break;
				case PedeSpawnEventData pedeSpawnEventData: _pedeSpawnEvents.Add(new EditorEvent(currentTick, entityId++, pedeSpawnEventData)); break;
				case SpiderEggSpawnEventData spiderEggSpawnEventData: _spiderEggSpawnEvents.Add(new EditorEvent(currentTick, entityId++, spiderEggSpawnEventData)); break;
				case SpiderSpawnEventData spiderSpawnEventData: _spiderSpawnEvents.Add(new EditorEvent(currentTick, entityId++, spiderSpawnEventData)); break;
				case SquidSpawnEventData squidSpawnEventData: _squidSpawnEvents.Add(new EditorEvent(currentTick, entityId++, squidSpawnEventData)); break;
				case ThornSpawnEventData thornSpawnEventData: _thornSpawnEvents.Add(new EditorEvent(currentTick, entityId++, thornSpawnEventData)); break;
				case TransmuteEventData transmuteEventData: _transmuteEvents.Add(new EditorEvent(currentTick, null, transmuteEventData)); break;
			}
		}
	}

	#endregion Factory methods

	#region Conversion methods

	public ReplayBinary<LocalReplayBinaryHeader> ToLocalReplay()
	{
		LocalReplayBinaryHeader header = new(
			Version,
			TimestampSinceGameRelease,
			Time,
			StartTime,
			DaggersFired,
			DeathType,
			Gems,
			DaggersHit,
			Kills,
			PlayerId,
			Username,
			new byte[10],
			Spawnset.ToBytes());

		return new ReplayBinary<LocalReplayBinaryHeader>(header, Cache.Events);
	}

	public byte[] ToHash()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		bw.Write(LookSpeed);
		bw.Write(InputsEvents.Count);
		for (int i = 0; i < InputsEvents.Count; i++)
			InputsEvents[i].Write(bw);

		WriteList(bw, _boidSpawnEvents);
		WriteList(bw, _daggerSpawnEvents);
		WriteList(bw, _entityOrientationEvents);
		WriteList(bw, _entityPositionEvents);
		WriteList(bw, _entityTargetEvents);
		WriteList(bw, _gemEvents);
		WriteList(bw, _hitEvents);
		WriteList(bw, _leviathanSpawnEvents);
		WriteList(bw, _pedeSpawnEvents);
		WriteList(bw, _spiderEggSpawnEvents);
		WriteList(bw, _spiderSpawnEvents);
		WriteList(bw, _squidSpawnEvents);
		WriteList(bw, _thornSpawnEvents);
		WriteList(bw, _transmuteEvents);

		return MD5.HashData(ms.ToArray());

		static void WriteList(BinaryWriter bw, List<EditorEvent> events)
		{
			bw.Write(events.Count);
			for (int i = 0; i < events.Count; i++)
				events[i].Data.Write(bw);
		}
	}

	#endregion Conversion methods

	#region Cache building

	private EditorReplayModelCache RebuildCache()
	{
		List<ReplayEvent> replayEvents = [];
		List<EntityType> entityTypes = [];

		List<EditorEvent> allEvents = _boidSpawnEvents
			.Concat(_daggerSpawnEvents)
			.Concat(_entityOrientationEvents)
			.Concat(_entityPositionEvents)
			.Concat(_entityTargetEvents)
			.Concat(_gemEvents)
			.Concat(_hitEvents)
			.Concat(_leviathanSpawnEvents)
			.Concat(_pedeSpawnEvents)
			.Concat(_spiderEggSpawnEvents)
			.Concat(_spiderSpawnEvents)
			.Concat(_squidSpawnEvents)
			.Concat(_thornSpawnEvents)
			.Concat(_transmuteEvents)
			.OrderBy(ee => ee.TickIndex)
			.ThenBy(ee => ee.EntityId)
			.ToList();

		int currentTickIndex = 0;
		foreach (EditorEvent editorEvent in allEvents)
		{
			// Check if we need to end the current tick with an inputs event.
			while (currentTickIndex < editorEvent.TickIndex)
			{
				if (currentTickIndex >= InputsEvents.Count)
					throw new InvalidOperationException("Current tick index is higher than the amount of inputs events.");

				InputsEventData inputsEvent = InputsEvents[currentTickIndex];
				if (currentTickIndex == 0)
					replayEvents.Add(new ReplayEvent(new InitialInputsEventData(inputsEvent.Left, inputsEvent.Right, inputsEvent.Forward, inputsEvent.Backward, inputsEvent.Jump, inputsEvent.Shoot, inputsEvent.ShootHoming, inputsEvent.MouseX, inputsEvent.MouseY, LookSpeed)));
				else
					replayEvents.Add(new ReplayEvent(inputsEvent));

				currentTickIndex++;
			}

			if (editorEvent.Data is ISpawnEventData spawnEventData)
				entityTypes.Add(spawnEventData.EntityType);

			replayEvents.Add(new ReplayEvent(editorEvent.Data));
		}

		replayEvents.Add(new ReplayEvent(new EndEventData()));

		int currentEntityId = 1;
		Dictionary<int, int> entityIdByEventIndex = new();
		for (int i = 0; i < replayEvents.Count; i++)
		{
			if (replayEvents[i].Data is ISpawnEventData)
				entityIdByEventIndex.Add(i, currentEntityId++);
		}

		return new EditorReplayModelCache(replayEvents, entityTypes, entityIdByEventIndex);
	}

	#endregion Cache building

	#region Event building

	private void InvalidateCache()
	{
		_cache = null;
	}

	public void AddEvent<T>(int tickIndex, T eventData)
		where T : IEventData
	{
		EventType eventType = eventData switch
		{
			BoidSpawnEventData => EventType.BoidSpawn,
			LeviathanSpawnEventData => EventType.LeviathanSpawn,
			PedeSpawnEventData => EventType.PedeSpawn,
			SpiderEggSpawnEventData => EventType.SpiderEggSpawn,
			SpiderSpawnEventData => EventType.SpiderSpawn,
			SquidSpawnEventData => EventType.SquidSpawn,
			ThornSpawnEventData => EventType.ThornSpawn,
			DaggerSpawnEventData => EventType.DaggerSpawn,
			EntityOrientationEventData => EventType.EntityOrientation,
			EntityPositionEventData => EventType.EntityPosition,
			EntityTargetEventData => EventType.EntityTarget,
			GemEventData => EventType.Gem,
			HitEventData => EventType.Hit,
			TransmuteEventData => EventType.Transmute,
			InitialInputsEventData => throw new UnreachableException($"Event type not supported by timeline editor: {eventData.GetType()}"),
			InputsEventData => throw new UnreachableException($"Event type not supported by timeline editor: {eventData.GetType()}"),
			EndEventData => throw new UnreachableException($"Event type not supported by timeline editor: {eventData.GetType()}"),
			_ => throw new UnreachableException($"Unknown event data type: {eventData.GetType()}"),
		};

		int? nextEntityId = GetNextEntityId(tickIndex, eventType);

		// Shift entity ids of events with higher entity ids than the added event.
		if (nextEntityId.HasValue)
			ShiftEntityIds(nextEntityId.Value, 1);

		List<EditorEvent> listOfEvents = GetEventsOfType(eventType);
		listOfEvents.Add(new EditorEvent(tickIndex, nextEntityId, eventData));

		InvalidateCache();
	}

	public void RemoveEvent(EditorEvent editorEvent)
	{
		switch (editorEvent.Data)
		{
			case BoidSpawnEventData: _boidSpawnEvents.Remove(editorEvent); break;
			case DaggerSpawnEventData: _daggerSpawnEvents.Remove(editorEvent); break;
			case EntityOrientationEventData: _entityOrientationEvents.Remove(editorEvent); break;
			case EntityPositionEventData: _entityPositionEvents.Remove(editorEvent); break;
			case EntityTargetEventData: _entityTargetEvents.Remove(editorEvent); break;
			case GemEventData: _gemEvents.Remove(editorEvent); break;
			case HitEventData: _hitEvents.Remove(editorEvent); break;
			case LeviathanSpawnEventData: _leviathanSpawnEvents.Remove(editorEvent); break;
			case PedeSpawnEventData: _pedeSpawnEvents.Remove(editorEvent); break;
			case SpiderEggSpawnEventData: _spiderEggSpawnEvents.Remove(editorEvent); break;
			case SpiderSpawnEventData: _spiderSpawnEvents.Remove(editorEvent); break;
			case SquidSpawnEventData: _squidSpawnEvents.Remove(editorEvent); break;
			case ThornSpawnEventData: _thornSpawnEvents.Remove(editorEvent); break;
			case TransmuteEventData: _transmuteEvents.Remove(editorEvent); break;
		}

		// Shift entity ids of events with higher entity ids than the removed event.
		if (editorEvent.EntityId.HasValue)
			ShiftEntityIds(editorEvent.EntityId.Value, -1);

		InvalidateCache();
	}

	private void ShiftEntityIds(int aboveEntityId, int increment)
	{
		List<EditorEvent> events = GetAllEvents();
		foreach (EditorEvent e in events)
		{
			if (e.EntityId >= aboveEntityId)
				e.EntityId += increment;

			switch (e.Data)
			{
				case BoidSpawnEventData boidSpawnEventData when boidSpawnEventData.SpawnerEntityId >= aboveEntityId:
					boidSpawnEventData.SpawnerEntityId += increment;
					break;
				case SpiderEggSpawnEventData spiderEggSpawnEventData when spiderEggSpawnEventData.SpawnerEntityId >= aboveEntityId:
					spiderEggSpawnEventData.SpawnerEntityId += increment;
					break;
				case EntityOrientationEventData entityOrientationEventData when entityOrientationEventData.EntityId >= aboveEntityId:
					entityOrientationEventData.EntityId += increment;
					break;
				case EntityPositionEventData entityPositionEventData when entityPositionEventData.EntityId >= aboveEntityId:
					entityPositionEventData.EntityId += increment;
					break;
				case EntityTargetEventData entityTargetEventData when entityTargetEventData.EntityId >= aboveEntityId:
					entityTargetEventData.EntityId += increment;
					break;
				case TransmuteEventData transmuteEventData when transmuteEventData.EntityId >= aboveEntityId:
					transmuteEventData.EntityId += increment;
					break;
				case HitEventData hitEventData:
				{
					if (hitEventData.EntityIdA >= aboveEntityId)
						hitEventData.EntityIdA += increment;
					if (hitEventData.EntityIdB >= aboveEntityId)
						hitEventData.EntityIdB += increment;
					break;
				}
			}
		}

		List<EditorEvent> GetAllEvents()
		{
			return BoidSpawnEvents
				.Concat(DaggerSpawnEvents)
				.Concat(EntityOrientationEvents)
				.Concat(EntityPositionEvents)
				.Concat(EntityTargetEvents)
				.Concat(GemEvents)
				.Concat(HitEvents)
				.Concat(LeviathanSpawnEvents)
				.Concat(PedeSpawnEvents)
				.Concat(SpiderEggSpawnEvents)
				.Concat(SpiderSpawnEvents)
				.Concat(SquidSpawnEvents)
				.Concat(ThornSpawnEvents)
				.Concat(TransmuteEvents)
				.ToList();
		}
	}

	private List<EditorEvent> GetEventsOfType(EventType eventType)
	{
		return eventType switch
		{
			EventType.BoidSpawn => _boidSpawnEvents,
			EventType.LeviathanSpawn => _leviathanSpawnEvents,
			EventType.PedeSpawn => _pedeSpawnEvents,
			EventType.SpiderEggSpawn => _spiderEggSpawnEvents,
			EventType.SpiderSpawn => _spiderSpawnEvents,
			EventType.SquidSpawn => _squidSpawnEvents,
			EventType.ThornSpawn => _thornSpawnEvents,
			EventType.DaggerSpawn => _daggerSpawnEvents,
			EventType.EntityOrientation => _entityOrientationEvents,
			EventType.EntityPosition => _entityPositionEvents,
			EventType.EntityTarget => _entityTargetEvents,
			EventType.Gem => _gemEvents,
			EventType.Hit => _hitEvents,
			EventType.Transmute => _transmuteEvents,
			_ => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
		};
	}

	// TODO: Remove; this approach is too slow.
	private List<EditorEvent> GetEventsAtTick(int tickIndex)
	{
		return BoidSpawnEvents
			.Concat(DaggerSpawnEvents)
			.Concat(EntityOrientationEvents)
			.Concat(EntityPositionEvents)
			.Concat(EntityTargetEvents)
			.Concat(GemEvents)
			.Concat(HitEvents)
			.Concat(LeviathanSpawnEvents)
			.Concat(PedeSpawnEvents)
			.Concat(SpiderEggSpawnEvents)
			.Concat(SpiderSpawnEvents)
			.Concat(SquidSpawnEvents)
			.Concat(ThornSpawnEvents)
			.Concat(TransmuteEvents)
			.Where(e => e.TickIndex == tickIndex)
			.ToList();
	}

	private int? GetNextEntityId(int tickIndex, EventType eventType)
	{
		// In case of enemy and dagger spawns, we need to determine the entity id of the new event.
		if (eventType is not (EventType.BoidSpawn or EventType.LeviathanSpawn or EventType.PedeSpawn or EventType.SpiderEggSpawn or EventType.SpiderSpawn or EventType.SquidSpawn or EventType.ThornSpawn or EventType.DaggerSpawn))
			return null;

		int entityId = 1;
		for (int i = tickIndex; i >= 0; i--)
		{
			// Find all spawn events with this tick index.
			// If there are none, continue the loop until the first tick.
			// If we find one, use that entityId + 1.
			// If we find multiple, use the highest entityId + 1.
			// TODO: This is very slow and should be optimized.
			int? highestEntityId = HighestEntityIdInList(GetEventsAtTick(i));
			if (highestEntityId.HasValue)
			{
				entityId = highestEntityId.Value + 1;
				break;
			}

			static int? HighestEntityIdInList(List<EditorEvent> events)
			{
				if (events.Count == 0)
					return null;

				int? highestEntityId = null;
				for (int i = 0; i < events.Count; i++)
				{
					if (!events[i].EntityId.HasValue)
						continue;

					if (!highestEntityId.HasValue || events[i].EntityId > highestEntityId)
						highestEntityId = events[i].EntityId;
				}

				return highestEntityId;
			}
		}

		return entityId;
	}

	#endregion Event building

	#region Entity id

	/// <summary>
	/// Returns the entity type of the entity with the specified entity ID, or <see langword="null" /> if a valid entity type could not be resolved.
	/// </summary>
	public EntityType? GetEntityType(int entityId)
	{
		if (entityId == 0)
			return EntityType.Zero;

		if (entityId < 0 || entityId > Cache.Entities.Count)
			return null;

		return Cache.Entities[entityId - 1];
	}

	/// <summary>
	/// Returns the entity type of the entity with the specified entity ID, or <see langword="null" /> if a valid entity type could not be resolved.
	/// This includes negated entity IDs that are used in <see cref="HitEventData"/> when a dead pede segment is hit.
	/// </summary>
	public EntityType? GetEntityTypeIncludingNegated(int entityId)
	{
		int absoluteEntityId = Math.Abs(entityId);
		if (absoluteEntityId > Cache.Entities.Count)
			return null;

		EntityType? entityType = GetEntityType(absoluteEntityId);
		if (entityId < 0 && entityType is not EntityType.Centipede and not EntityType.Gigapede and not EntityType.Ghostpede)
			return null;

		return entityType;
	}

	#endregion Entity id
}
