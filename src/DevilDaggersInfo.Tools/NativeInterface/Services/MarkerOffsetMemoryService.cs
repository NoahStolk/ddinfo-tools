using System.Diagnostics;

namespace DevilDaggersInfo.Tools.NativeInterface.Services;

/// <summary>
/// Base class for the platforms that locate the ddstats block by reading a pointer the game stores at a fixed offset
/// from its main module, which the DevilDaggers.info API supplies. Only reading, writing, and finding the process are
/// platform-specific; turning the marker offset into a block address is the same wherever the offset exists.
/// </summary>
internal abstract class MarkerOffsetMemoryService : INativeMemoryService
{
	private readonly byte[] _pointerBuffer = new byte[sizeof(long)];

	/// <inheritdoc />
	public bool RequiresMarkerOffset => true;

	/// <inheritdoc />
	public BlockAddressResult ResolveBlockAddress(Process process, long? ddstatsMarkerOffset)
	{
		if (process.MainModule == null || ddstatsMarkerOffset is not { } markerOffset)
			return BlockAddressResult.Unresolved;

		_pointerBuffer.AsSpan().Clear();
		ReadMemory(process, process.MainModule.BaseAddress.ToInt64() + markerOffset, _pointerBuffer, 0, sizeof(long));
		return BlockAddressResult.Resolved(BitConverter.ToInt64(_pointerBuffer));
	}

	/// <inheritdoc />
	public abstract void WriteMemory(Process process, long address, byte[] bytes, int offset, int size);

	/// <inheritdoc />
	public abstract void ReadMemory(Process process, long address, byte[] bytes, int offset, int size);

	/// <inheritdoc />
	public abstract Process? GetDevilDaggersProcess();
}
