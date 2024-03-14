namespace DevilDaggersInfo.Tools.GameMemory.Enemies;

public sealed record EnemyMemory
{
	public EnemyMemory(int baseAddress, int structSize, IReadOnlyList<int> offsets)
	{
		BaseAddress = baseAddress;
		StructSize = structSize;
		Offsets = offsets;
	}

	public int BaseAddress { get; }
	public int StructSize { get; }
	public IReadOnlyList<int> Offsets { get; }
}
