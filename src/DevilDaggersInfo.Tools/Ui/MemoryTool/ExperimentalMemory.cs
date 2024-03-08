using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.Ui.MemoryTool;

public static class ExperimentalMemory
{
	private static readonly List<Thorn> _thorns = [];
	private static readonly List<Spider> _spiders = [];
	private static readonly List<Leviathan> _leviathans = [];
	private static readonly List<Squid> _squids = [];
	private static readonly List<Pede> _pedes = [];
	private static readonly List<Boid> _boids = [];

	private static float _recordingTimer;

	public static float ScanInterval = 0.25f;

	public static IReadOnlyList<Thorn> Thorns => _thorns;
	public static IReadOnlyList<Spider> Spiders => _spiders;
	public static IReadOnlyList<Leviathan> Leviathans => _leviathans;
	public static IReadOnlyList<Squid> Squids => _squids;
	public static IReadOnlyList<Pede> Pedes => _pedes;
	public static IReadOnlyList<Boid> Boids => _boids;

	public static void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < ScanInterval)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		MainBlock mainBlock = Root.GameMemoryService.MainBlock;
		int thornListLength = mainBlock.ThornAliveCount;
		int spiderListLength = mainBlock.Spider1AliveCount + mainBlock.Spider2AliveCount;
		int leviathanListLength = mainBlock.LeviathanAliveCount + mainBlock.OrbAliveCount;
		int squidListLength = mainBlock.Squid1AliveCount + mainBlock.Squid2AliveCount + mainBlock.Squid3AliveCount;
		int pedeListLength = mainBlock.CentipedeAliveCount + mainBlock.GigapedeAliveCount + mainBlock.GhostpedeAliveCount;
		int boidListLength = mainBlock.Skull1AliveCount + mainBlock.Skull2AliveCount + mainBlock.Skull3AliveCount + mainBlock.Skull4AliveCount + mainBlock.SpiderlingAliveCount;

		ReadEnemyList(_thorns, thornListLength, 0x002513B0, StructSizes.Thorn, [0x0, 0x28, 0]);
		ReadEnemyList(_spiders, spiderListLength, 0x00251830, StructSizes.Spider, [0x0, 0x28, 0]);
		ReadEnemyList(_leviathans, leviathanListLength, 0x00251590, StructSizes.Leviathan, [0x0, 0x28, 0]);
		ReadEnemyList(_squids, squidListLength, 0x00251890, StructSizes.Squid, [0x0, 0x18, 0]);
		ReadEnemyList(_pedes, pedeListLength, 0x00251470, StructSizes.Pede, [0x0, 0x28, 0]);
		ReadEnemyList(_boids, boidListLength, 0x00251410, StructSizes.Boid, [0x0, 0x20, 0]);

		for (int i = 0; i < _squids.Count; i++)
		{
			Squid squid = _squids[i];
			if (squid.GushCountDown < -2)
			{
				squid.GushCountDown = -2;
				int gushCountDownOffset = (int)Marshal.OffsetOf<Squid>(nameof(Squid.GushCountDown));
				Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + gushCountDownOffset], squid.GushCountDown);
			}
		}

		for (int i = 0; i < _boids.Count; i++)
		{
			Boid boid = _boids[i];
			if (boid.Timer < 1.0f)
			{
				boid.Hp = boid.Type switch
				{
					BoidType.Skull1 => 0,
					// BoidType.Skull2 => 0,
					// BoidType.Skull3 => 0,
					// BoidType.Skull4 => 0,
					// BoidType.Spiderling => 0,
					_ => boid.Hp,
				};
				//
				// boid.BaseSpeed = boid.Type switch
				// {
				// 	BoidType.Skull1 => 1,
				// 	BoidType.Skull2 => 20,
				// 	BoidType.Skull3 => 4,
				// 	BoidType.Skull4 => 4,
				// 	BoidType.Spiderling => 1,
				// 	_ => boid.BaseSpeed,
				// };
				//
				// boid.Type = boid.Type switch
				// {
				// 	_ => boid.Type,
				// };

				// Only write the necessary data back into memory, otherwise we're sending outdated data back into memory.
				int hpOffset = (int)Marshal.OffsetOf<Boid>(nameof(Boid.Hp));
				int speedOffset = (int)Marshal.OffsetOf<Boid>(nameof(Boid.BaseSpeed));
				int typeOffset = (int)Marshal.OffsetOf<Boid>(nameof(Boid.Type));
				Root.GameMemoryService.WriteExperimental(0x00251410, [0x0, 0x20, 0x00 + hpOffset + i * StructSizes.Boid], boid.Hp);
				Root.GameMemoryService.WriteExperimental(0x00251410, [0x0, 0x20, 0x00 + speedOffset + i * StructSizes.Boid], boid.BaseSpeed);
				Root.GameMemoryService.WriteExperimental(0x00251410, [0x0, 0x20, 0x00 + typeOffset + i * StructSizes.Boid], (short)boid.Type);
			}
		}
	}

	private static void ReadEnemyList<TEnemy>(List<TEnemy> list, int count, long address, int size, Span<int> offsets)
		where TEnemy : unmanaged
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			byte[] buffer = Root.GameMemoryService.ReadExperimental(address, size, offsets);
			TEnemy enemy = MemoryMarshal.Read<TEnemy>(buffer);
			list.Add(enemy);
			offsets[^1] += size;
		}
	}

	public static void KillLeviathan()
	{
		for (int i = 0; i < 6; i++)
			Root.GameMemoryService.WriteExperimental(0x00251590, [0x0, 0x28, 0x84 + i * 56], 0);
	}

	public static void KillOrb()
	{
		Root.GameMemoryService.WriteExperimental(0x00251590, [0x0, 0x28, 0x84 + 6 * 56 + 8], 0);
	}

	public static void KillAllSquids()
	{
		for (int i = 0; i < Root.GameMemoryService.MainBlock.Squid1AliveCount + Root.GameMemoryService.MainBlock.Squid2AliveCount + Root.GameMemoryService.MainBlock.Squid3AliveCount; i++)
		{
			Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 148], 0);
			Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 152], 0);
			Root.GameMemoryService.WriteExperimental(0x00251890, [0x0, 0x18, i * StructSizes.Squid + 156], 0);
		}
	}
}
