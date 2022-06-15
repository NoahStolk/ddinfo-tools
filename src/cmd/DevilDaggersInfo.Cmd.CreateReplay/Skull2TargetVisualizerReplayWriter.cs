namespace DevilDaggersInfo.Cmd.CreateReplay;

public class Skull2TargetVisualizerReplayWriter : IReplayWriter
{
	public ReplayBinary Write()
	{
		ReplayBinary original = new(File.ReadAllBytes(Path.Combine("Resources", "Replays", "Skull2Analysis.ddreplay")), ReplayBinaryReadComprehensiveness.All);
		if (original.SpawnsetBuffer == null || original.CompressedEvents == null)
			throw new InvalidOperationException("Must read replay binary entirely by setting ReplayBinaryReadComprehensiveness to All.");

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

		return new(
			version: original.Version,
			timestampSinceGameRelease: original.TimestampSinceGameRelease,
			time: original.Time,
			startTime: original.StartTime,
			daggersFired: original.DaggersFired,
			deathType: original.DeathType,
			gems: original.Gems,
			daggersHit: original.DaggersHit,
			kills: original.Kills,
			playerId: original.PlayerId,
			username: original.Username,
			spawnsetBuffer: original.SpawnsetBuffer,
			compressedEvents: ReplayEventsParser.CompileEvents(newEvents));
	}

	private static void AddVisualizer(List<IEvent> newEvents, int visualizerEntityId, EntityTargetEvent targetEvent)
	{
		for (int i = 0; i < 10; i++)
		{
			newEvents.Add(new DaggerSpawnEvent(visualizerEntityId, 0, targetEvent.Position, Int16Mat3x3.Identity, 0, 1));
		}
	}
}
