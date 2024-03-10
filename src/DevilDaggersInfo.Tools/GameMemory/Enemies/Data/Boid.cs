using DevilDaggersInfo.Core.Replay.Numerics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.BoidSize)]
public record struct Boid : IEnemy
{
	[FieldOffset(000)] public BoidType Type;
	[FieldOffset(002)] public short Hp;
	[FieldOffset(004)] public int SpawnerId;
	[FieldOffset(008)] public Vector3 Position;
	[FieldOffset(020)] public Vector3 Velocity;
	[FieldOffset(032)] public float BaseSpeed;
	[FieldOffset(036)] public Matrix3x3 Rotation;
	[FieldOffset(072)] public Matrix4x4 Floats;
	[FieldOffset(200)] public float Timer;

	public bool IsValid()
	{
		bool validType = Type is BoidType.Skull1 or BoidType.Skull2 or BoidType.Skull3 or BoidType.Skull4 or BoidType.Spiderling;
		bool validHp = Hp >= 0;
		bool validSpawnerId = SpawnerId > 0;
		bool validSpeed = BaseSpeed >= 0;
		return validType && validSpawnerId && validHp && validSpeed;
	}
}
#pragma warning restore SA1134
