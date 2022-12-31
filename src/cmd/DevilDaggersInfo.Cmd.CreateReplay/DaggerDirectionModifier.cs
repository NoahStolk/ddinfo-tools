using DevilDaggersInfo.Core.Replay.Events.Interfaces;

namespace DevilDaggersInfo.Cmd.CreateReplay;

public class DaggerDirectionModifier : IReplayWriter
{
	public ReplayBinary<LocalReplayBinaryHeader> Write()
	{
		ReplayBinary<LocalReplayBinaryHeader> original = new(File.ReadAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\PSY_DJUMP.ddreplay"));
		List<IEvent> newEvents = new();

		HashSet<int> daggerIds = new();

		foreach (IEvent e in original.EventsData.Events)
		{
			if (e is DaggerSpawnEvent daggerSpawnEvent)
			{
				Matrix4x4 newDirection = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2f);
				newEvents.Add(daggerSpawnEvent with { Orientation = Int16Mat3x3.FromMatrix4x4(newDirection) });

				daggerIds.Add(daggerSpawnEvent.EntityId);
			}
			else if (e is EntityOrientationEvent entityOrientationEvent && daggerIds.Contains(entityOrientationEvent.EntityId))
			{
				Matrix4x4 newDirection = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2f);
				newEvents.Add(entityOrientationEvent with { Orientation = Int16Mat3x3.FromMatrix4x4(newDirection) });
			}
			else
			{
				newEvents.Add(e);
			}
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
}
