using DevilDaggersInfo.Core.Replay.Events.Interfaces;
using DevilDaggersInfo.Types.Core.Replays;

// File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\TEST.ddreplay", new RandomReplayWriter().Write().Compile());
// File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\skull-2-analysis-done_113.72-xvlv-0be4d3b8[2d05a83a].ddreplay", new Skull2TargetVisualizerReplayWriter().Write().Compile());
// File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\deaths_1.00-xvlv-0be4d3b8[2d05a83a].ddreplay", new DeathsReplayWriter().Write().Compile());
// File.WriteAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\DJUMPMODIFIED.ddreplay", new DaggerDirectionModifier().Write().Compile());

ReplayBinary<LocalReplayBinaryHeader> original = new(File.ReadAllBytes(@"C:\Users\NOAH\AppData\Roaming\DevilDaggers\replays\PSY_DJUMP.ddreplay"));

int tick = 0;
int consecutiveHoldCount = 0;
int consecutiveNoneCount = 0;
foreach (IInputsEvent e in original.EventsData.Events.OfType<IInputsEvent>())
{
	if (e.Shoot == ShootType.None)
	{
		consecutiveNoneCount++;
	}
	else if (e.Shoot == ShootType.Hold)
	{
		consecutiveHoldCount++;
	}
	else if (e.Shoot == ShootType.Release)
	{
		Console.WriteLine($"Not using LMB for {consecutiveNoneCount} ticks");
		Console.WriteLine($"Holding LMB for   {consecutiveHoldCount} ticks");
		Console.WriteLine($"Released at       {tick / 60f:00.0000} seconds");
		Console.WriteLine("===");

		consecutiveNoneCount = 0;
		consecutiveHoldCount = 0;
	}

	tick++;
}
