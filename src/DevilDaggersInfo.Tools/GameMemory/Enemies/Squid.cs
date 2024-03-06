using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Squid)]
public record struct Squid
{
	[FieldOffset(0)] public int Hp;
}
#pragma warning restore SA1134
