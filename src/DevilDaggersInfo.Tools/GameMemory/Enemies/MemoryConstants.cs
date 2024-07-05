using DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

public static class MemoryConstants
{
	private const int _pedeSegmentCount = 50;
	private const int _pedeSegmentSize = 352;

	public const int ThornSize = 2288;
	public const int SpiderSize = 632;
	public const int LeviathanSize = 480;
	public const int SquidSize = 608;
	public const int PedeSize = 6984 + _pedeSegmentCount * _pedeSegmentSize;
	public const int BoidSize = 520;

	public static EnemyMemory Thorn { get; } = new(0x002513B0, ThornSize, new[] { 0x0, 0x28, 0x0 });
	public static EnemyMemory Spider { get; } = new(0x00251830, SpiderSize, new[] { 0x0, 0x28, 0x0 });
	public static EnemyMemory Leviathan { get; } = new(0x00251590, LeviathanSize, new[] { 0x0, 0x28, 0x0 });
	public static EnemyMemory Squid { get; } = new(0x00251890, SquidSize, new[] { 0x0, 0x18, 0x0 });
	public static EnemyMemory Pede { get; } = new(0x00251470, PedeSize + _pedeSegmentCount * _pedeSegmentSize, new[] { 0x0, 0x28, 0x0 });
	public static EnemyMemory Boid { get; } = new(0x00251410, BoidSize, new[] { 0x0, 0x20, 0x0 });

	public static EnemyMemory GetEnemyMemory<TEnemy>()
		where TEnemy : unmanaged
	{
		return typeof(TEnemy) switch
		{
			Type thornType when thornType == typeof(Thorn) => Thorn,
			Type spiderType when spiderType == typeof(Spider) => Spider,
			Type leviathanType when leviathanType == typeof(Leviathan) => Leviathan,
			Type squidType when squidType == typeof(Squid) => Squid,
			Type pedeType when pedeType == typeof(Pede) => Pede,
			Type boidType when boidType == typeof(Boid) => Boid,
			_ => throw new ArgumentException($"Unknown enemy type {typeof(TEnemy).Name}."),
		};
	}
}
