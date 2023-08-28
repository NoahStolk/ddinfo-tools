using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;

namespace DevilDaggersInfo.Cmd.CreateReplay;

public class NaNBoids : IReplayWriter
{
	public ReplayBinary<LocalReplayBinaryHeader> Write()
	{
		ReplayBinary<LocalReplayBinaryHeader> original = new(File.ReadAllBytes(/*Path.Combine("Resources", "Replays", "Skull2Analysis.ddreplay")*/@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\doublesquid_429.12-LocoCaesarIV-0bed0c5b.ddreplay"));
		List<IEvent> newEvents = new();

		foreach (IEvent e in original.EventsData.Events)
		{
			if (e is EntityOrientationEvent entityOrientationEvent &&
			    original.EventsData.EntityTypes[entityOrientationEvent.EntityId] is EntityType.Skull1 or EntityType.Skull2 &&
			    entityOrientationEvent.Orientation == default)
			{
				newEvents.Add(entityOrientationEvent with
				{
					Orientation = Int16Mat3x3.Identity,
				});
			}
			else if (e is EntityTargetEvent entityTargetEvent &&
				original.EventsData.EntityTypes[entityTargetEvent.EntityId] is EntityType.Skull1 or EntityType.Skull2 &&
				entityTargetEvent.TargetPosition == default)
			{
				newEvents.Add(entityTargetEvent with
				{
					TargetPosition = new Int16Vec3(0, 0, 1),
				});
			}
			else if (e is BoidSpawnEvent { SpawnerEntityId: 5580 } boidSpawnEvent)
			{
				newEvents.Add(boidSpawnEvent with
				{
					Position = boidSpawnEvent.Position + new Int16Vec3(0, 0, 1),
				});
			}
			else
			{
				newEvents.Add(e);
			}
		}

		LocalReplayBinaryHeader header = new(
			version: original.Header.Version,
			timestampSinceGameRelease: LocalReplayBinaryHeader.GetTimestampSinceGameReleaseFromDateTimeOffset(DateTimeOffset.UtcNow),
			time: original.Header.Time,
			startTime: original.Header.StartTime,
			daggersFired: original.Header.DaggersFired,
			deathType: original.Header.DeathType,
			gems: original.Header.Gems,
			daggersHit: original.Header.DaggersHit,
			kills: original.Header.Kills,
			playerId: original.Header.PlayerId,
			username: original.Header.Username,
			unknown: original.Header.Unknown,
			spawnsetBuffer: original.Header.SpawnsetBuffer);

		return new(
			header: header,
			compressedEvents: ReplayEventsCompiler.CompileEvents(newEvents));
	}
}
