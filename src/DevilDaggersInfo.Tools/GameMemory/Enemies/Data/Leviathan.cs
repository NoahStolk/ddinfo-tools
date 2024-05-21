using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.LeviathanSize)]
public record struct Leviathan
{
	[FieldOffset(000)] public LeviathanState State;
	// [FieldOffset(004)] another state? 0 beckoning, 2 idle, 3 preparing beckon??
	[FieldOffset(132)] public int NodeHp1;
	[FieldOffset(188)] public int NodeHp2;
	[FieldOffset(244)] public int NodeHp3;
	[FieldOffset(300)] public int NodeHp4;
	[FieldOffset(356)] public int NodeHp5;
	[FieldOffset(412)] public int NodeHp6;
	[FieldOffset(476)] public int OrbHp;
}
#pragma warning restore SA1134
