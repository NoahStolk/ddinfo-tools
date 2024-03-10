using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.GameMemory.Enemies.Data;

#pragma warning disable SA1134
[StructLayout(LayoutKind.Explicit, Size = MemoryConstants.SpiderSize)]
public record struct Spider : IEnemy
{
	// [FieldOffset(000)] public int IsHiding;
	// [FieldOffset(004)] public int IsNotHiding; // ??
	[FieldOffset(228)] public int Hp;

	// TODO: SpiderType int enum.

	public bool IsValid()
	{
		bool validHp = Hp >= 0;
		return validHp;
	}
}
#pragma warning restore SA1134
