using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class TechSpeed
{
	public static List<Entry> ProcessReplay(ReplayEventsData replay)
	{
		List<Entry> entries = new();

		int ticksShootNone = 0;
		int ticksShootHold = 0;

		int daggersThisTick = 0;

		int tick = 0;

		for (int i = 0; i < replay.Events.Count; i++)
		{
			ReplayEvent replayEvent = replay.Events[i];
			if (replayEvent.Data is DaggerSpawnEventData)
			{
				daggersThisTick++;
			}
			else if (replayEvent.Data is InitialInputsEventData)
			{
				tick++;
			}
			else if (replayEvent.Data is InputsEventData inputs)
			{
				tick++;

				if (inputs.Shoot == ShootType.None)
				{
					ticksShootNone++;
				}
				else if (inputs.Shoot == ShootType.Hold)
				{
					ticksShootHold++;
				}
				else
				{
					entries.Add(new(tick, daggersThisTick, ticksShootNone, ticksShootHold));

					ticksShootNone = 0;
					ticksShootHold = 0;
				}

				daggersThisTick = 0;
			}
		}

		return entries;
	}

	public sealed record Entry(int Tick, int DaggersFired, int TicksNone, int TicksHold);
}
