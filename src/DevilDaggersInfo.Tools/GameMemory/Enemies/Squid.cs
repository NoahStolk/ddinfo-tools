using System.Numerics;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Squid)]
public record struct Squid
{
	[FieldOffset(008)] public float Timer;
	[FieldOffset(012)] public Vector3 Position;
	[FieldOffset(148)] public int HpNode1;
	[FieldOffset(152)] public int HpNode2;
	[FieldOffset(156)] public int HpNode3;
	[FieldOffset(172)] public float GushCountDown;
	[FieldOffset(240)] public SquidType Type;
}
#pragma warning restore SA1134
