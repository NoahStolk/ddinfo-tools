using DevilDaggersInfo.Tools;
using DevilDaggersInfo.Tools.GameMemory.Enemies.Data;
using DevilDaggersInfo.Tools.Ui.MemoryTool;

for (int i = 0; i < ExperimentalMemory.Boids.Count; i++)
{
	Boid boid = ExperimentalMemory.Boids[i];
	if (boid.Timer < 1.0f)
	{
		if (boid.Type == BoidType.Skull1)
		{
			ExperimentalMemory.WriteValue<Boid, short>(i, nameof(Boid.Hp), 4);
			ExperimentalMemory.WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), 18);
		}

		if (boid.Type == BoidType.Skull2)
		{
			ExperimentalMemory.WriteValue<Boid, short>(i, nameof(Boid.Hp), 100);
			ExperimentalMemory.WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), 10);
		}
	}
}

for (int i = 0; i < ExperimentalMemory.Squids.Count; i++)
{
	Squid squid = ExperimentalMemory.Squids[i];
	if (squid.GushCountDown < -0.25f)
		ExperimentalMemory.WriteValue<Squid, float>(i, nameof(Squid.GushCountDown), -0.25f);

	if (Root.GameMemoryService.MainBlock.Time > 4)
		ExperimentalMemory.WriteValue<Squid, int>(i, nameof(Squid.NodeHp1), 0);
}
