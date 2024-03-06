using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Squid)]
public record struct Squid
{
	[FieldOffset(0)] public int HpNode1;
	[FieldOffset(4)] public int HpNode2;
	[FieldOffset(8)] public int HpNode3;
}
#pragma warning restore SA1134
