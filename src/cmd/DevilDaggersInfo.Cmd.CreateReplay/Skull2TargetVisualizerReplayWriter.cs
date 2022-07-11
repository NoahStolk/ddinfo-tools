namespace DevilDaggersInfo.Cmd.CreateReplay;

public class Skull2TargetVisualizerReplayWriter : IReplayWriter
{
	public ReplayBinary Write()
	{
		ReplayBinary original = new(File.ReadAllBytes(Path.Combine("Resources", "Replays", "Skull2Analysis.ddreplay")));
		List<IEvent> originalEvents = ReplayEventsParser.ParseCompressedEvents(original.CompressedEvents).SelectMany(e => e).ToList();
		List<IEvent> newEvents = new();
		int skull2EntityId = -1;

		int visualizerEntityId = 100;
		foreach (IEvent e in originalEvents)
		{
			if (skull2EntityId == -1 && e is BoidSpawnEvent boidSpawn && boidSpawn.BoidType == BoidType.Skull2)
				skull2EntityId = boidSpawn.EntityId;

			if (e is EntityTargetEvent targetEvent && targetEvent.EntityId == skull2EntityId)
			{
				AddVisualizer(newEvents, visualizerEntityId, targetEvent);
				visualizerEntityId++;
			}

			newEvents.Add(e);
		}

		ReplayBinaryHeader header = new(
			version: original.Header.Version,
			timestampSinceGameRelease: original.Header.TimestampSinceGameRelease,
			time: original.Header.Time,
			startTime: original.Header.StartTime,
			daggersFired: original.Header.DaggersFired,
			deathType: original.Header.DeathType,
			gems: original.Header.Gems,
			daggersHit: original.Header.DaggersHit,
			kills: original.Header.Kills,
			playerId: original.Header.PlayerId,
			username: original.Header.Username,
			spawnsetBuffer: original.Header.SpawnsetBuffer);

		return new(
			header: header,
			compressedEvents: ReplayEventsParser.CompileEvents(newEvents));
	}

	private static void AddVisualizer(List<IEvent> newEvents, int visualizerEntityId, EntityTargetEvent targetEvent)
	{
		for (int i = 0; i < 10; i++)
		{
			newEvents.Add(new DaggerSpawnEvent(visualizerEntityId, 0, targetEvent.Position with { Y = 50 }, new(158, -11, 201, -201, 3, -159, 5, -256, -10), 0, 1));
		}
	}
}
