using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = 348)]
public record struct Leviathan
{
	[FieldOffset(0)] public int NodeHp1;
	[FieldOffset(56)] public int NodeHp2;
	[FieldOffset(112)] public int NodeHp3;
	[FieldOffset(168)] public int NodeHp4;
	[FieldOffset(224)] public int NodeHp5;
	[FieldOffset(280)] public int NodeHp6;
	[FieldOffset(344)] public int OrbHp;
}
#pragma warning restore SA1134
