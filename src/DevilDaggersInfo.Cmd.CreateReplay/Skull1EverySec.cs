using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Events.Interfaces;

namespace DevilDaggersInfo.Cmd.CreateReplay;

public class Skull1EverySec : IReplayWriter
{
	public ReplayBinary<LocalReplayBinaryHeader> Write()
	{
		List<IEvent> events = new()
		{
			new HitEvent(353333333, 353333333, 353333333),
			new InitialInputsEvent(false, false, false, false, JumpType.None, ShootType.None, ShootType.None, 0, 0, 0.005f),
		};

		int id = 1;
		for (int i = 0; i < 1200; i++)
		{
			if (i == 0)
				events.Add(new SquidSpawnEvent(id++, SquidType.Squid1, -1, new(8, 8, 8), Vector3.UnitX, 10));

			if (i == 30)
				events.Add(new SquidSpawnEvent(id++, SquidType.Squid1, -1, new(-8, 8, -8), Vector3.UnitX, 10));

			if (i % 60 == 0)
			{
				events.Add(new BoidSpawnEvent(id++, 1, BoidType.Skull1, new(20, 20, 20), Int16Mat3x3.Identity, Vector3.UnitZ, 16));
				events.Add(new BoidSpawnEvent(id++, 2, BoidType.Skull1, new(20, 20, 20), Int16Mat3x3.Identity, Vector3.UnitZ, 16));
			}

			EndTick(Movement.None, i == 0 || i > 740 && i % 52 == 0 ? JumpType.StartedPress : JumpType.None, ShootType.None, ShootType.None, i < 90 || i is > 430 and < 465 ? 8 : i > 740 ? 6 : 0, i > 800 ? (int)Math.Sin(i / 6f) * 15 : 0);
		}

		events.Add(new EndEvent());

		byte[] spawnsetBuffer = File.ReadAllBytes(Path.Combine("Resources", "Spawnsets", "EmptySpawnset"));
		LocalReplayBinaryHeader header = new(1, 2, events.Count(e => e is InputsEvent) / 60f, 0, 0, 0, 0, 0, 0, 999999, "test", new byte[10], spawnsetBuffer);
		return new(header, ReplayEventsCompiler.CompileEvents(events));

		void EndTick(Movement movement, JumpType jump, ShootType lmb, ShootType rmb, int mouseX, int mouseY)
		{
			events.Add(new InputsEvent(movement.HasFlag(Movement.Left), movement.HasFlag(Movement.Right), movement.HasFlag(Movement.Forward), movement.HasFlag(Movement.Backward), jump, lmb, rmb, (short)mouseX, (short)mouseY));
		}
	}
}
