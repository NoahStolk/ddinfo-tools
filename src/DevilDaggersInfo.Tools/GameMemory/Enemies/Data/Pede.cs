using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.PedeSize)]
public record struct Pede : IEnemy
{
	[FieldOffset(6984)] public PedeSegments Segments;

	public bool IsValid()
	{
		bool validHp = Segments.Segment0.Hp >= 0;
		return validHp;
	}
}
#pragma warning restore SA1134
