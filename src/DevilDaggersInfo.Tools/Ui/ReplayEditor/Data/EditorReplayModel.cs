using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public record EditorReplayModel
{
	private readonly List<EntityType> _entityIds = [];

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

	// Data typically found in replay headers.
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

	// Embedded inputs data.
	public float LookSpeed { get; set; }
	public List<InputsEventData> InputsEvents { get; } = [];

	// All other events.
	public List<EditorEvent> BoidSpawnEvents { get; } = [];
	public List<EditorEvent> DaggerSpawnEvents { get; } = [];
	public List<EditorEvent> EntityOrientationEvents { get; } = [];
	public List<EditorEvent> EntityPositionEvents { get; } = [];
	public List<EditorEvent> EntityTargetEvents { get; } = [];
	public List<EditorEvent> GemEvents { get; } = [];
	public List<EditorEvent> HitEvents { get; } = [];
	public List<EditorEvent> LeviathanSpawnEvents { get; } = [];
	public List<EditorEvent> PedeSpawnEvents { get; } = [];
	public List<EditorEvent> SpiderEggSpawnEvents { get; } = [];
	public List<EditorEvent> SpiderSpawnEvents { get; } = [];
	public List<EditorEvent> SquidSpawnEvents { get; } = [];
	public List<EditorEvent> ThornSpawnEvents { get; } = [];
	public List<EditorEvent> TransmuteEvents { get; } = [];

	public static EditorReplayModel CreateDefault()
	{
		return new(
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

	public static EditorReplayModel CreateFromLeaderboardReplay(int playerId, string username, ReplayEventsData replayEventsData)
	{
		EditorReplayModel replay = CreateDefault();
		replay.PlayerId = playerId;
		replay.Username = username;
		replay.AddReplayEventsData(replayEventsData);
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
		replay.AddReplayEventsData(localReplay.EventsData);
		return replay;
	}

	private void AddReplayEventsData(ReplayEventsData replayEventsData)
	{
		int currentTick = 0;
		foreach (ReplayEvent replayEvent in replayEventsData.Events)
		{
			if (replayEvent.Data is ISpawnEventData spawnEventData)
			{
				_entityIds.Add(spawnEventData.EntityType);
			}

			switch (replayEvent.Data)
			{
				case InputsEventData inputsEventData:
					InputsEvents.Add(inputsEventData);
					currentTick++;
					break;
				case InitialInputsEventData initialInputsEventData:
					InputsEvents.Add(new(
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
				case BoidSpawnEventData boidSpawnEventData: BoidSpawnEvents.Add(new(currentTick, boidSpawnEventData)); break;
				case DaggerSpawnEventData daggerSpawnEventData: DaggerSpawnEvents.Add(new(currentTick, daggerSpawnEventData)); break;
				case EntityOrientationEventData entityOrientationEventData: EntityOrientationEvents.Add(new(currentTick, entityOrientationEventData)); break;
				case EntityPositionEventData entityPositionEventData: EntityPositionEvents.Add(new(currentTick, entityPositionEventData)); break;
				case EntityTargetEventData entityTargetEventData: EntityTargetEvents.Add(new(currentTick, entityTargetEventData)); break;
				case GemEventData gemEventData: GemEvents.Add(new(currentTick, gemEventData)); break;
				case HitEventData hitEventData: HitEvents.Add(new(currentTick, hitEventData)); break;
				case LeviathanSpawnEventData leviathanSpawnEventData: LeviathanSpawnEvents.Add(new(currentTick, leviathanSpawnEventData)); break;
				case PedeSpawnEventData pedeSpawnEventData: PedeSpawnEvents.Add(new(currentTick, pedeSpawnEventData)); break;
				case SpiderEggSpawnEventData spiderEggSpawnEventData: SpiderEggSpawnEvents.Add(new(currentTick, spiderEggSpawnEventData)); break;
				case SpiderSpawnEventData spiderSpawnEventData: SpiderSpawnEvents.Add(new(currentTick, spiderSpawnEventData)); break;
				case SquidSpawnEventData squidSpawnEventData: SquidSpawnEvents.Add(new(currentTick, squidSpawnEventData)); break;
				case ThornSpawnEventData thornSpawnEventData: ThornSpawnEvents.Add(new(currentTick, thornSpawnEventData)); break;
				case TransmuteEventData transmuteEventData: TransmuteEvents.Add(new(currentTick, transmuteEventData)); break;
			}
		}
	}

	/// <summary>
	/// Returns the entity type of the entity with the specified entity ID, or <see langword="null" /> if a valid entity type could not be resolved.
	/// </summary>
	public EntityType? GetEntityType(int entityId)
	{
		if (entityId == 0)
			return EntityType.Zero;

		if (entityId < 0 || entityId > _entityIds.Count)
			return null;

		return _entityIds[entityId - 1];
	}

	/// <summary>
	/// Returns the entity type of the entity with the specified entity ID, or <see langword="null" /> if a valid entity type could not be resolved.
	/// This includes negated entity IDs that are used in <see cref="HitEventData"/> when a dead pede segment is hit.
	/// </summary>
	public EntityType? GetEntityTypeIncludingNegated(int entityId)
	{
		int absoluteEntityId = Math.Abs(entityId);
		if (absoluteEntityId > _entityIds.Count)
			return null;

		EntityType? entityType = GetEntityType(absoluteEntityId);
		if (entityId < 0 && entityType is not EntityType.Centipede and not EntityType.Gigapede and not EntityType.Ghostpede)
			return null;

		return entityType;
	}

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		bw.Write("DDINFO__REPLAY__TIMELINE"u8);

		bw.Write(LookSpeed);
		bw.Write(InputsEvents.Count);
		for (int i = 0; i < InputsEvents.Count; i++)
			InputsEvents[i].Write(bw);

		WriteList(bw, BoidSpawnEvents);
		WriteList(bw, DaggerSpawnEvents);
		WriteList(bw, EntityOrientationEvents);
		WriteList(bw, EntityPositionEvents);
		WriteList(bw, EntityTargetEvents);
		WriteList(bw, GemEvents);
		WriteList(bw, HitEvents);
		WriteList(bw, LeviathanSpawnEvents);
		WriteList(bw, PedeSpawnEvents);
		WriteList(bw, SpiderEggSpawnEvents);
		WriteList(bw, SpiderSpawnEvents);
		WriteList(bw, SquidSpawnEvents);
		WriteList(bw, ThornSpawnEvents);
		WriteList(bw, TransmuteEvents);

		return ms.ToArray();

		static void WriteList(BinaryWriter bw, List<EditorEvent> events)
		{
			bw.Write(events.Count);
			for (int i = 0; i < events.Count; i++)
				events[i].Data.Write(bw);
		}
	}
}