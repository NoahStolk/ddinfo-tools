using DevilDaggersInfo.Core.Replay.Numerics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Boid)]
public record struct Boid
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
}
#pragma warning restore SA1134
