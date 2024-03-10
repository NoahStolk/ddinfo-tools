using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.LeviathanSize)]
public record struct Leviathan : IEnemy
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

	public bool IsValid()
	{
		bool validState = State is LeviathanState.PreparingBeckon or LeviathanState.Beckoning or LeviathanState.Idle;
		bool validHp = NodeHp1 >= 0 && NodeHp2 >= 0 && NodeHp3 >= 0 && NodeHp4 >= 0 && NodeHp5 >= 0 && NodeHp6 >= 0 && OrbHp >= 0;
		return validState && validHp;
	}
}
#pragma warning restore SA1134
