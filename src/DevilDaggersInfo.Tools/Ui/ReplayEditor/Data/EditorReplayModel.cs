using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Spawnset;

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

	// TODO: Invalidate cache when any of the below properties are changed or values are added/removed/moved from lists.

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

	private ReplayEventsData CompileEventsData()
	{
		ReplayEventsData replayEventsData = new();

		for (int i = 0; i < InputsEvents.Count; i++)
		{
			AddEvents(BoidSpawnEvents);
			AddEvents(DaggerSpawnEvents);
			AddEvents(EntityOrientationEvents);
			AddEvents(EntityPositionEvents);
			AddEvents(EntityTargetEvents);
			AddEvents(GemEvents);
			AddEvents(HitEvents);
			AddEvents(LeviathanSpawnEvents);
			AddEvents(PedeSpawnEvents);
			AddEvents(SpiderEggSpawnEvents);
			AddEvents(SpiderSpawnEvents);
			AddEvents(SquidSpawnEvents);
			AddEvents(ThornSpawnEvents);
			AddEvents(TransmuteEvents);

			if (i == 0)
				replayEventsData.AddEvent(new InitialInputsEventData(InputsEvents[i].Left, InputsEvents[i].Right, InputsEvents[i].Forward, InputsEvents[i].Backward, InputsEvents[i].Jump, InputsEvents[i].Shoot, InputsEvents[i].ShootHoming, InputsEvents[i].MouseX, InputsEvents[i].MouseY, LookSpeed));
			else
				replayEventsData.AddEvent(InputsEvents[i]);

			void AddEvents(List<EditorEvent> events)
			{
				for (int j = 0; j < events.Count; j++)
				{
					EditorEvent editorEvent = events[j];
					if (editorEvent.TickIndex == i)
						replayEventsData.AddEvent(editorEvent.Data);
				}
			}
		}

		replayEventsData.AddEvent(new EndEventData());

		return replayEventsData;
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
