using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.GameMemory.Enemies;
using DevilDaggersInfo.Tools.GameMemory.Enemies.Data;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
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

	public static float ScanInterval = 1 / 60f;

	public static IReadOnlyList<Thorn> Thorns => _thorns;
	public static IReadOnlyList<Spider> Spiders => _spiders;
	public static IReadOnlyList<Leviathan> Leviathans => _leviathans;
	public static IReadOnlyList<Squid> Squids => _squids;
	public static IReadOnlyList<Pede> Pedes => _pedes;
	public static IReadOnlyList<Boid> Boids => _boids;

	public static void Update(float delta)
	{
		if (!SurvivalFileWatcher.Exists)
			return; // Do not run for default V3 spawnset.

		_recordingTimer += delta;
		if (_recordingTimer < ScanInterval)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		// TODO: Don't get this from the main block. Find the actual list lengths.
		// - Use spawnset with 16 thorns.
		// - Find 16.
		// - Kill 1 thorn.
		// - Find 16 until the thorn is removed from memory (should take a couple seconds after death).
		// - Then find 15.
		// - Repeat until list length is found.
		// Could also try with spawning a new thorn every 10 seconds and see if the list length increases.
		// Or try this with 40 leviathans.
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

		// TODO: This method shouldn't be present in the window.
		ScriptingWindow.RunScript();

#if PEDE_WEIRDNESS
		for (int i = 0; i < Pedes.Count; i++)
		{
			for (int j = 0; j < 1; j++)
			{
				// Change some random stuff for the first seg.
				// Change the HP to 1 always, to spawn infinite gems when shooting pedes.
				WriteValue<Pede, int>(i, 6984 + j * 352, 1); // HP
				//WriteValue<Pede, int>(i, 6984 + j * 352 + 4 * 2, 1); // get stuck
				WriteValue<Pede, int>(i, 6984 + j * 352 + 4 * 12, 1); // homing explosion

				// 12 as float crashes the game?
				// for (int k = 13; k < 80; k++)
				// {
				// 	WriteValue<Pede, int>(i, 6984 + j * 352 + 4 * k, 1); // not sure
				// }
			}
		}
#endif

#if INFINITE_GEM_SQUID_DEAD_AFTER_10
		for (int i = 0; i < _squids.Count; i++)
		{
			int hp = Root.GameMemoryService.MainBlock.Time > 10 ? 0 : 1;

			// Changing the HP to 1 constantly spawns gems whenever a dagger hits the squid. This works for Leviathan and pedes as well.
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp1), hp);
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp2), hp);
			WriteValue<Squid, int>(i, nameof(Squid.NodeHp3), hp);
		}
#endif

#if OBSTACLE_COURSE
		for (int i = 0; i < _thorns.Count; i++)
		{
			WriteValue<Thorn, int>(i, nameof(Thorn.Hp), 1);
			float timer = Root.GameMemoryService.MainBlock.Time + i;
			float sin = MathF.Sin(timer * 0.15f * MathF.PI) * 20;
			float cos = MathF.Cos(timer * 0.23f * MathF.PI) * 20;
			WriteValue<Thorn, Vector3>(i, nameof(Thorn.Position), new Vector3(sin, 0, cos));
			//WriteValue<Thorn, float>(i, nameof(Thorn.Rotation), Root.GameMemoryService.MainBlock.Time);
		}
#endif

#if SKULL_RING
		for (int i = 0; i < _boids.Count; i++)
		{
			Boid boid = _boids[i];

			float timer = boid.Timer + i * 0.1f;
			float sin = MathF.Sin(timer * 0.25f * MathF.PI) * 25;
			float y = (MathF.Sin(timer * 0.6f * MathF.PI) + 1.3f) * 3;
			float cos = MathF.Cos(timer * 0.25f * MathF.PI) * 25;
			WriteValue<Boid, Vector3>(i, nameof(Boid.Position), new Vector3(sin, y, cos));
			float angle = MathF.Atan2(sin, cos);
			Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle + MathF.PI * 0.5f) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Root.GameMemoryService.MainBlock.Time * 20 + i * 0.25f);
			Matrix4x4 mat = Matrix4x4.CreateFromQuaternion(rotation);
			Matrix3x3 rot = new(mat.M11, mat.M12, mat.M13, mat.M21, mat.M22, mat.M23, mat.M31, mat.M32, mat.M33);
			WriteValue<Boid, Matrix3x3>(i, nameof(Boid.Rotation), rot);
			WriteValue<Boid, Vector3>(i, nameof(Boid.Velocity), new Vector3(0, 1, 0));
			WriteValue<Boid, Matrix4x4>(i, nameof(Boid.Floats), Matrix4x4.Identity);
			WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), 2);
			WriteValue<Boid, Vector3>(i, nameof(Boid.Velocity), new Vector3(2));
		}
#endif

		// for (int i = 0; i < ExperimentalMemory.Pedes.Count; i++)
		// {
		// 	for (int j = 0; j < 50; j++)
		// 	{
		// 		// Idk what happens here but there's a lot of homing explosions happening.
		// 		WriteValue<Pede, float>(i, 6984 + j * 4, 1);
		// 	}
		// }

#if TEST1
		// Only write the necessary data back into memory, otherwise we're sending outdated data back into memory.
		for (int i = 0; i < _thorns.Count; i++)
		{
			Thorn thorn = _thorns[i];
			if (thorn.Hp > 10)
			{
				WriteValue<Thorn, int>(i, nameof(Thorn.Hp), 10);
				// WriteValue<Thorn, float>(i, nameof(Thorn.Rotation), Root.GameMemoryService.MainBlock.Time);
			}
		}

		for (int i = 0; i < _squids.Count; i++)
		{
			Squid squid = _squids[i];
			if (squid.GushCountDown < -0.25f)
				WriteValue<Squid, float>(i, nameof(Squid.GushCountDown), -0.25f);

			if (mainBlock.Time > 4)
				WriteValue<Squid, int>(i, nameof(Squid.NodeHp1), 0);
		}

		for (int i = 0; i < _boids.Count; i++)
		{
			Boid boid = _boids[i];
			if (boid.Timer < 1.0f)
			{
				// boid.Hp = boid.Type switch
				// {
				// 	BoidType.Skull1 => 0,
					// BoidType.Skull2 => 0,
					// BoidType.Skull3 => 0,
					// BoidType.Skull4 => 0,
					// BoidType.Spiderling => 0,
				// 	_ => boid.Hp,
				// };
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

				if (boid.Type == BoidType.Skull1)
				{
					WriteValue<Boid, short>(i, nameof(Boid.Hp), 4);
					WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), 0);
				}

				if (boid.Type == BoidType.Skull2)
				{
					WriteValue<Boid, short>(i, nameof(Boid.Hp), 100);
					WriteValue<Boid, float>(i, nameof(Boid.BaseSpeed), 0);
				}
			}
		}
#endif
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
			TEnemy enemy = ReadEnemy<TEnemy>(enemyMemory, offsets);
			list.Add(enemy);
			offsets[^1] += enemyMemory.StructSize;
		}
	}

	private static TEnemy ReadEnemy<TEnemy>(EnemyMemory enemyMemory, Span<int> offsets)
		where TEnemy : unmanaged
	{
		Span<byte> buffer = stackalloc byte[enemyMemory.StructSize];
		Root.GameMemoryService.ReadExperimental(enemyMemory.BaseAddress, buffer, offsets);
		return MemoryMarshal.Read<TEnemy>(buffer);
	}

	// This is called dynamically by scripts.
	// ReSharper disable once MemberCanBePrivate.Global
	public static void WriteValue<TEnemy, TValue>(int index, string fieldName, TValue value)
		where TEnemy : unmanaged
		where TValue : unmanaged
	{
		int fieldOffset = (int)Marshal.OffsetOf<TEnemy>(fieldName);
		WriteValue<TEnemy, TValue>(index, fieldOffset, value);
	}

	// This is called dynamically by scripts.
	// ReSharper disable once MemberCanBePrivate.Global
	public static void WriteValue<TEnemy, TValue>(int index, int fieldOffset, TValue value)
		where TEnemy : unmanaged
		where TValue : unmanaged
	{
		EnemyMemory enemyMemory = MemoryConstants.GetEnemyMemory<TEnemy>();

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
