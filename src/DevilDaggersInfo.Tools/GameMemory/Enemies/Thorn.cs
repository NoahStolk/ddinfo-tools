using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Thorn)]
public record struct Thorn
{
	[FieldOffset(00)] public ThornState State;
	[FieldOffset(04)] public int IsAlive;
	[FieldOffset(08)] public float StateTimer;
	[FieldOffset(12)] public int Hp;
	[FieldOffset(16)] public Vector3 Position;
	[FieldOffset(28)] public float Rotation;
	[FieldOffset(32)] public float Unknown1;
	[FieldOffset(36)] public float Unknown2;
	[FieldOffset(40)] public float Unknown3;
	[FieldOffset(44)] public float Unknown4;
	[FieldOffset(48)] public float Unknown5;
	[FieldOffset(52)] public float Unknown6;
	[FieldOffset(56)] public float Unknown7;
	[FieldOffset(60)] public float Unknown8;
	[FieldOffset(64)] public float Unknown9;
	[FieldOffset(68)] public float Unknown10;
	[FieldOffset(72)] public float Unknown11;
}
#pragma warning restore SA1134
