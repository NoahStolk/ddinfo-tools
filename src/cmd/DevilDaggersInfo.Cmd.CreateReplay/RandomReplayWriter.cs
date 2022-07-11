using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevilDaggersInfo.Cmd.CreateReplay;

public class RandomReplayWriter : IReplayWriter
{
	public ReplayBinary Write()
	{
		List<IEvent> events = new();
		events.Add(new HitEvent(353333333, 353333333, 353333333));
		events.Add(new InitialInputsEvent(false, false, false, false, 0, false, false, 0, 0, 0.005f));

		for (int i = 0; i < 1200; i++)
		{
			//if (i % 20 == 0)
			//events.Add(new DaggerSpawnEvent(entityId++, 3, new(5, 14, 5), Int16Mat3x3.Identity, 3, 1));

			if (i == 30)
				events.Add(new SquidSpawnEvent(1, SquidType.Squid2, -1, new(0, 0, 20), new(0, 2, 0), -549.1759f));

			if (i >= 60 && i <= 70)
				events.Add(new BoidSpawnEvent(i - 58, 1, BoidType.Skull2, new(20, 20, 20), Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Vector3.Zero, 4));

			if (i == 80)
			{
				for (int j = 0; j < 100; j++)
					events.Add(new DaggerSpawnEvent(20000 + j, -1, new((short)Math.Sin(j), 30, (short)Math.Cos(j)), Int16Mat3x3.Identity, true, 6));
			}

			if (i == 170)
				events.Add(new PedeSpawnEvent(333, PedeType.Ghostpede, -1, new(0, 70, 0), Vector3.Zero, Matrix3x3.Identity));

			if (i == 200)
			{
				for (int j = 0; j < 10; j++)
					events.Add(new EntityTargetEvent(j + 2, new((short)(-20 - i), 20, -20)));
			}

			if (i == 400)
			{
				for (int j = 0; j < 10; j++)
					events.Add(new TransmuteEvent(j + 2, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero));
			}

			if (i is 450 or 460 or 470)
				events.Add(new SpiderSpawnEvent(666, SpiderType.Spider1, -1, new((i - 540) / 4f, (i - 370) / 4f, -15 + (i - 460) / 2f)));

			if (i > 530 && i < 630)
			{
				if (i < 570 && i % 4 == 0)
					events.Add(new GemEvent());
				else if (i >= 570)
					events.Add(new GemEvent());
			}

			if (i == 666)
			{
				for (int j = 0; j < 149; j++)
					events.Add(new GemEvent());
			}

			if (i == 690)
				events.Add(new GemEvent());

			//if (i >= 510 && i < 540)
			//	events.Add(new SpiderEggSpawnEvent(i, 666, new(-16.389393f, 11.098727f, -4.125659f), new(26.663002f, -17.759218f, 7.9157248f)));

			if (i == 720)
			{
				events.Add(new BoidSpawnEvent(1000, 1, BoidType.Skull4, new(-250, 20, -250), Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Vector3.Zero, 10000));
				events.Add(new BoidSpawnEvent(1001, 1, BoidType.Skull4, new(-250, 150, -280), Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Vector3.Zero, 10000));
				events.Add(new BoidSpawnEvent(1002, 1, BoidType.Skull4, new(-250, 20, -310), Int16Vec3.Zero, Int16Vec3.Zero, Int16Vec3.Zero, Vector3.Zero, 10000));
			}

			if (i == 780)
			{
				for (int j = 0; j < 100; j++)
					events.Add(new HitEvent(1000, 20000 + j, 0));
			}

			Movement movement = Movement.None;
			if (i < 30)
				movement |= Movement.Forward;

			if (i > 740)
			{
				movement |= Movement.Backward;
				movement |= Movement.Left;
			}

			EndTick(movement, i == 0 || i > 740 && i % 52 == 0 ? JumpType.PreciseHop : JumpType.None, false, false, i < 90 || i > 430 && i < 465 ? 8 : i > 740 ? 6 : 0, i > 800 ? (int)Math.Sin(i / 6f) * 15 : 0);
		}

		events.Add(new EndEvent());

		byte[] spawnsetBuffer = File.ReadAllBytes(Path.Combine("Resources", "Spawnsets", "EmptySpawnset"));
		ReplayBinaryHeader header = new(1, 2, events.Count(e => e is InputsEvent) / 60f, 0, 0, 0, 0, 0, 0, 999999, "test", spawnsetBuffer);
		return new(header, ReplayEventsParser.CompileEvents(events));

		void EndTick(Movement movement, JumpType jump, bool lmb, bool rmb, int mouseX, int mouseY)
		{
			events.Add(new InputsEvent(movement.HasFlag(Movement.Left), movement.HasFlag(Movement.Right), movement.HasFlag(Movement.Forward), movement.HasFlag(Movement.Backward), jump, lmb, rmb, (short)mouseX, (short)mouseY));
		}
	}
}
