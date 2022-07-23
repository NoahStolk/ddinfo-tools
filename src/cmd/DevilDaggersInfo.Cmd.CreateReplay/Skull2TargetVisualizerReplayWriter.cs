using DevilDaggersInfo.Core.Replay.Events.Interfaces;

namespace DevilDaggersInfo.Cmd.CreateReplay;

public class Skull2TargetVisualizerReplayWriter : IReplayWriter
{
	public ReplayBinary<LocalReplayBinaryHeader> Write()
	{
		ReplayBinary<LocalReplayBinaryHeader> original = new(File.ReadAllBytes(/*Path.Combine("Resources", "Replays", "Skull2Analysis.ddreplay")*/@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\111-ninja-skull2_335.77-Derkan-0c0e40f3.ddreplay"));
		List<IEvent> newEvents = new();

		int visualizerEntityId = 100;
		foreach (IEvent e in original.EventsData.Events)
		{
			if (e is EntityTargetEvent targetEvent && original.EventsData.EntityTypes[targetEvent.EntityId] == EntityType.Skull2)
			{
				// TODO: Shift all existing entity IDs forward.
				AddVisualizer(newEvents, visualizerEntityId, targetEvent);
				visualizerEntityId++;
			}

			newEvents.Add(e);
		}

		LocalReplayBinaryHeader header = new(
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
			compressedEvents: ReplayEventsCompiler.CompileEvents(newEvents));
	}

	private static void AddVisualizer(List<IEvent> newEvents, int visualizerEntityId, EntityTargetEvent targetEvent)
	{
		for (int i = 0; i < 10; i++)
		{
			newEvents.Add(new DaggerSpawnEvent(visualizerEntityId, 0, targetEvent.TargetPosition with { Y = 50 }, new(158, -11, 201, -201, 3, -159, 5, -256, -10), false, DaggerType.Level1));
		}
	}
}
