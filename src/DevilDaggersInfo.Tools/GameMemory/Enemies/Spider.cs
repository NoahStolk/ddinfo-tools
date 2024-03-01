using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = 632)]
public record struct Spider
{
	[FieldOffset(0)] public int Hp;

	// TODO: SpiderType int enum.
}
#pragma warning restore SA1134
