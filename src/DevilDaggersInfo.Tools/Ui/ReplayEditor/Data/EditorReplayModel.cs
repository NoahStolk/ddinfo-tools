using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public record EditorReplayModel
{
	private ReplayEventsData? _replayEventsDataCache;

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

	public int TickCount => InputsEvents.Count;

	public ReplayEventsData Cache => _replayEventsDataCache ??= CompileEventsData();

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
		int entityId = 1;
		foreach (IEventData eventData in replayEventsData.Events.Select(e => e.Data))
		{
			switch (eventData)
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
				case BoidSpawnEventData boidSpawnEventData: BoidSpawnEvents.Add(new(currentTick, entityId++, boidSpawnEventData)); break;
				case DaggerSpawnEventData daggerSpawnEventData: DaggerSpawnEvents.Add(new(currentTick, entityId++, daggerSpawnEventData)); break;
				case EntityOrientationEventData entityOrientationEventData: EntityOrientationEvents.Add(new(currentTick, null, entityOrientationEventData)); break;
				case EntityPositionEventData entityPositionEventData: EntityPositionEvents.Add(new(currentTick, null, entityPositionEventData)); break;
				case EntityTargetEventData entityTargetEventData: EntityTargetEvents.Add(new(currentTick, null, entityTargetEventData)); break;
				case GemEventData gemEventData: GemEvents.Add(new(currentTick, null, gemEventData)); break;
				case HitEventData hitEventData: HitEvents.Add(new(currentTick, null, hitEventData)); break;
				case LeviathanSpawnEventData leviathanSpawnEventData: LeviathanSpawnEvents.Add(new(currentTick, entityId++, leviathanSpawnEventData)); break;
				case PedeSpawnEventData pedeSpawnEventData: PedeSpawnEvents.Add(new(currentTick, entityId++, pedeSpawnEventData)); break;
				case SpiderEggSpawnEventData spiderEggSpawnEventData: SpiderEggSpawnEvents.Add(new(currentTick, entityId++, spiderEggSpawnEventData)); break;
				case SpiderSpawnEventData spiderSpawnEventData: SpiderSpawnEvents.Add(new(currentTick, entityId++, spiderSpawnEventData)); break;
				case SquidSpawnEventData squidSpawnEventData: SquidSpawnEvents.Add(new(currentTick, entityId++, squidSpawnEventData)); break;
				case ThornSpawnEventData thornSpawnEventData: ThornSpawnEvents.Add(new(currentTick, entityId++, thornSpawnEventData)); break;
				case TransmuteEventData transmuteEventData: TransmuteEvents.Add(new(currentTick, null, transmuteEventData)); break;
			}
		}
	}

	private ReplayEventsData CompileEventsData()
	{
		ReplayEventsData replayEventsData = new();

		List<EditorEvent> eventsThisTick = [];

		for (int i = 0; i < InputsEvents.Count; i++)
		{
			eventsThisTick.Clear();

			eventsThisTick.AddRange(BoidSpawnEvents
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
				.Where(e => e.TickIndex == i)
				.ToList());

			eventsThisTick.Sort((a, b) => (a.EntityId ?? -1).CompareTo(b.EntityId ?? -1));
			foreach (EditorEvent editorEvent in eventsThisTick)
				replayEventsData.AddEvent(editorEvent.Data);

			if (i == 0)
				replayEventsData.AddEvent(new InitialInputsEventData(InputsEvents[i].Left, InputsEvents[i].Right, InputsEvents[i].Forward, InputsEvents[i].Backward, InputsEvents[i].Jump, InputsEvents[i].Shoot, InputsEvents[i].ShootHoming, InputsEvents[i].MouseX, InputsEvents[i].MouseY, LookSpeed));
			else
				replayEventsData.AddEvent(InputsEvents[i]);
		}

		replayEventsData.AddEvent(new EndEventData());

		return replayEventsData;
	}

	public void AddEmptyEvent(int tickIndex, EventType eventType)
	{
		int entityId = 0;

		for (int i = tickIndex; i >= 0; i--)
		{
			// Find all spawn events with this tick index.
			// If there are none, continue.
			// If we find one, use that entityId + 1.
			// If we find multiple, use the highest entityId + 1.
			int? highestEntityId = HighestEntityIdInList(BoidSpawnEvents.Concat(DaggerSpawnEvents).Concat(LeviathanSpawnEvents).Concat(PedeSpawnEvents).Concat(SpiderEggSpawnEvents).Concat(SpiderSpawnEvents).Concat(SquidSpawnEvents).Concat(ThornSpawnEvents).ToList());
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

		Action action = eventType switch
		{
			EventType.BoidSpawn => () => BoidSpawnEvents.Add(new(tickIndex, entityId, BoidSpawnEventData.CreateDefault())),
			EventType.LeviathanSpawn => () => LeviathanSpawnEvents.Add(new(tickIndex, entityId, LeviathanSpawnEventData.CreateDefault())),
			EventType.PedeSpawn => () => PedeSpawnEvents.Add(new(tickIndex, entityId, PedeSpawnEventData.CreateDefault())),
			EventType.SpiderEggSpawn => () => SpiderEggSpawnEvents.Add(new(tickIndex, entityId, SpiderEggSpawnEventData.CreateDefault())),
			EventType.SpiderSpawn => () => SpiderSpawnEvents.Add(new(tickIndex, entityId, SpiderSpawnEventData.CreateDefault())),
			EventType.SquidSpawn => () => SquidSpawnEvents.Add(new(tickIndex, entityId, SquidSpawnEventData.CreateDefault())),
			EventType.ThornSpawn => () => ThornSpawnEvents.Add(new(tickIndex, entityId, ThornSpawnEventData.CreateDefault())),
			EventType.DaggerSpawn => () => DaggerSpawnEvents.Add(new(tickIndex, entityId, DaggerSpawnEventData.CreateDefault())),
			EventType.EntityOrientation => () => EntityOrientationEvents.Add(new(tickIndex, null, EntityOrientationEventData.CreateDefault())),
			EventType.EntityPosition => () => EntityPositionEvents.Add(new(tickIndex, null, EntityPositionEventData.CreateDefault())),
			EventType.EntityTarget => () => EntityTargetEvents.Add(new(tickIndex, null, EntityTargetEventData.CreateDefault())),
			EventType.Gem => () => GemEvents.Add(new(tickIndex, null, GemEventData.CreateDefault())),
			EventType.Hit => () => HitEvents.Add(new(tickIndex, null, HitEventData.CreateDefault())),
			EventType.Transmute => () => TransmuteEvents.Add(new(tickIndex, null, TransmuteEventData.CreateDefault())),
			EventType.InitialInputs => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
			EventType.Inputs => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
			EventType.End => throw new UnreachableException($"Event type not supported by timeline editor: {eventType}"),
			_ => throw new UnreachableException($"Unknown event type: {eventType}"),
		};

		action();
	}

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

		return new(header, Cache);
	}

	public byte[] ToHash()
	{
		// TODO: Make something faster.
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

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

		return MD5.HashData(ms.ToArray());

		static void WriteList(BinaryWriter bw, List<EditorEvent> events)
		{
			bw.Write(events.Count);
			for (int i = 0; i < events.Count; i++)
				events[i].Data.Write(bw);
		}
	}
}
