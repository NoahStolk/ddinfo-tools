namespace DevilDaggersInfo.Tools.NativeInterface.Services;

/// <summary>
/// Where the ddstats block is, or why it is not known.
/// </summary>
/// <param name="Status">The outcome of the resolution attempt.</param>
/// <param name="Address">The address of the block; only meaningful when <paramref name="Status" /> is <see cref="BlockAddressStatus.Resolved" />.</param>
internal readonly record struct BlockAddressResult(BlockAddressStatus Status, long Address)
{
	public static BlockAddressResult Unresolved { get; } = new(BlockAddressStatus.Unresolved, 0);

	public static BlockAddressResult Resolved(long address)
	{
		return new BlockAddressResult(BlockAddressStatus.Resolved, address);
	}
}
