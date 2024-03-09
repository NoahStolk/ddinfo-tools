using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using DevilDaggersInfo.Tools.GameMemory.Enemies.Data;
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

		// TODO: Don't get this from the main block. Find the actual list lengths.
		MainBlock mainBlock = Root.GameMemoryService.MainBlock;
		int thornListLength = mainBlock.ThornAliveCount;
		int spiderListLength = mainBlock.Spider1AliveCount + mainBlock.Spider2AliveCount;
		int leviathanListLength = mainBlock.LeviathanAliveCount + mainBlock.OrbAliveCount;
		int squidListLength = mainBlock.Squid1AliveCount + mainBlock.Squid2AliveCount + mainBlock.Squid3AliveCount;
		int pedeListLength = mainBlock.CentipedeAliveCount + mainBlock.GigapedeAliveCount + mainBlock.GhostpedeAliveCount;
		int boidListLength = mainBlock.Skull1AliveCount + mainBlock.Skull2AliveCount + mainBlock.Skull3AliveCount + mainBlock.Skull4AliveCount + mainBlock.SpiderlingAliveCount;

		ReadEnemyList(_thorns, thornListLength, MemoryConstants.Thorn);
		ReadEnemyList(_spiders, spiderListLength, MemoryConstants.Spider);
		ReadEnemyList(_leviathans, leviathanListLength, MemoryConstants.Leviathan);
		ReadEnemyList(_squids, squidListLength, MemoryConstants.Squid);
		ReadEnemyList(_pedes, pedeListLength, MemoryConstants.Pede);
		ReadEnemyList(_boids, boidListLength, MemoryConstants.Boid);

		for (int i = 0; i < _squids.Count; i++)
		{
			Squid squid = _squids[i];
			if (squid.GushCountDown < -2)
				WriteValue<Squid, float>(i, nameof(Squid.GushCountDown), -2);
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
				WriteValue<Boid, int>(i, nameof(Boid.Hp), boid.Hp);
				WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), boid.BaseSpeed);
				WriteValue<Boid, short>(i, nameof(Boid.Type), (short)boid.Type);
			}
		}
	}

	private static void ReadEnemyList<TEnemy>(List<TEnemy> list, int count, EnemyMemory enemyMemory)
		where TEnemy : unmanaged
	{
		list.Clear();

		Span<int> offsets = stackalloc int[enemyMemory.Offsets.Count];
		for (int i = 0; i < enemyMemory.Offsets.Count; i++)
			offsets[i] = enemyMemory.Offsets[i];

		for (int i = 0; i < count; i++)
		{
			byte[] buffer = Root.GameMemoryService.ReadExperimental(enemyMemory.BaseAddress, enemyMemory.StructSize, offsets);
			TEnemy enemy = MemoryMarshal.Read<TEnemy>(buffer);
			list.Add(enemy);
			offsets[^1] += enemyMemory.StructSize;
		}
	}

	private static void WriteValue<TEnemy, TValue>(int index, string fieldName, TValue value)
		where TEnemy : unmanaged
		where TValue : unmanaged
	{
		EnemyMemory enemyMemory = MemoryConstants.GetEnemyMemory<TEnemy>();
		int fieldOffset = (int)Marshal.OffsetOf<TEnemy>(fieldName);

		Span<int> offsets = stackalloc int[enemyMemory.Offsets.Count];
		for (int i = 0; i < enemyMemory.Offsets.Count; i++)
			offsets[i] = enemyMemory.Offsets[i];

		offsets[^1] += index * enemyMemory.StructSize + fieldOffset;

		Root.GameMemoryService.WriteExperimental(enemyMemory.BaseAddress, offsets, value);
	}

	public static void KillLeviathan()
	{
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp1), 0);
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp2), 0);
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp3), 0);
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp4), 0);
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp5), 0);
		WriteValue<Leviathan, int>(0, nameof(Leviathan.NodeHp6), 0);
	}

	public static void KillOrb()
	{
		WriteValue<Leviathan, int>(0, nameof(Leviathan.OrbHp), 0);
	}

	public static void KillAllSquids()
	{
		for (int i = 0; i < _squids.Count; i++)
		{
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp1), 0);
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp2), 0);
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp3), 0);
		}
	}
}
