namespace DevilDaggersInfo.Cmd.CreateReplay;

public class DeathsReplayWriter : IReplayWriter
{
	public ReplayBinary Write()
	{
		List<IEvent> events = new();
		events.Add(new HitEvent(353333333, 353333333, 353333333));
		events.Add(new InitialInputsEvent(false, false, false, false, 0, false, false, 0, 0, 0.005f));

		for (int i = 0; i < 60; i++)
		{
			if (i == 30)
				events.Add(new DeathEvent(1));

			if (i == 45)
				events.Add(new InputsEvent(false, false, false, false, JumpType.None, true, false, 0, 0));
			else
				events.Add(new InputsEvent(false, false, false, false, JumpType.None, false, false, 0, 0));
		}

		events.Add(new EndEvent());

		byte[] spawnsetBuffer = File.ReadAllBytes(Path.Combine("Resources", "Spawnsets", "EmptySpawnset"));
		ReplayBinaryHeader header = new(
			version: 1,
			timestampSinceGameRelease: 0,
			time: 1,
			startTime: 0,
			daggersFired: 0,
			deathType: 1,
			gems: 0,
			daggersHit: 0,
			kills: 0,
			playerId: 0,
			username: "Deaths test 2",
			spawnsetBuffer: spawnsetBuffer);

		return new(
			header: header,
			compressedEvents: ReplayEventsParser.CompileEvents(events));
	}

	private static void AddVisualizer(List<IEvent> newEvents, int visualizerEntityId, EntityTargetEvent targetEvent)
	{
		for (int i = 0; i < 10; i++)
		{
			newEvents.Add(new DaggerSpawnEvent(visualizerEntityId, 0, targetEvent.TargetPosition with { Y = 50 }, new(158, -11, 201, -201, 3, -159, 5, -256, -10), false, DaggerType.Level1));
		}
	}
}