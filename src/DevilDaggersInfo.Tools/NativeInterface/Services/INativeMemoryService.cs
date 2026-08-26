using System.Diagnostics;

namespace DevilDaggersInfo.Tools.NativeInterface.Services;

internal interface INativeMemoryService
{
	/// <summary>
	/// Whether this platform needs the ddstats marker offset from the DevilDaggers.info API in order to locate the
	/// block. Platforms that search the game's memory for the block have no offset to fetch, and no API route to fetch
	/// one from.
	/// </summary>
	bool RequiresMarkerOffset { get; }

	void WriteMemory(Process process, long address, byte[] bytes, int offset, int size);

	void ReadMemory(Process process, long address, byte[] bytes, int offset, int size);

	Process? GetDevilDaggersProcess();

	/// <summary>
	/// Resolves the address of the ddstats block inside <paramref name="process" />. How that is done is the platform's
	/// business: reading a pointer at a known offset, or searching the game's memory for the block itself.
	/// </summary>
	/// <param name="process">The running game.</param>
	/// <param name="ddstatsMarkerOffset">
	/// The marker offset from the API, or <see langword="null" /> when <see cref="RequiresMarkerOffset" /> is
	/// <see langword="false" /> and there is therefore nothing to fetch.
	/// </param>
	BlockAddressResult ResolveBlockAddress(Process process, long? ddstatsMarkerOffset);
}
