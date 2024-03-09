using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = 352)]
public record struct PedeSegment
{
	[FieldOffset(0)] public int Hp;
}
#pragma warning restore SA1134
