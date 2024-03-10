using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.SquidSize)]
public record struct Squid : IEnemy
{
	[FieldOffset(008)] public float Timer;
	[FieldOffset(012)] public Vector3 Position;
	[FieldOffset(148)] public int NodeHp1;
	[FieldOffset(152)] public int NodeHp2;
	[FieldOffset(156)] public int NodeHp3;
	[FieldOffset(172)] public float GushCountDown;
	[FieldOffset(240)] public SquidType Type;

	public bool IsValid()
	{
		bool validType = Type is SquidType.Squid1 or SquidType.Squid2 or SquidType.Squid3;
		bool validHp = NodeHp1 >= 0 && NodeHp2 >= 0 && NodeHp3 >= 0;
		return validType && validHp;
	}
}
#pragma warning restore SA1134
