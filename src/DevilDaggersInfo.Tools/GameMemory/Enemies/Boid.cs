using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = StructSizes.Boid)]
public record struct Boid
{
	[FieldOffset(0)] public short Hp;
}
#pragma warning restore SA1134
