using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.GameMemory;

internal sealed class GameMemoryService(INativeMemoryService nativeMemoryService)
{
	public const int StatsBufferSize = 112;

	private readonly byte[] _mainBuffer = new byte[MainBlock.Size];
	private readonly byte[] _replayIdentifierBuffer = new byte[LocalReplayBinaryHeader.IdentifierLength];

	private long _memoryBlockAddress;
	private Process? _process;

	public MainBlock MainBlockPrevious { get; private set; }
	public MainBlock MainBlock { get; private set; }

	public bool IsInitialized { get; private set; }

	/// <summary>
	/// Whether <see cref="Initialize" /> needs the ddstats marker offset from the API on this platform.
	/// </summary>
	public bool RequiresMarkerOffset => nativeMemoryService.RequiresMarkerOffset;

	/// <summary>
	/// Why the address of the ddstats block is or is not known, so a game whose memory is off limits can be told apart
	/// from a game that simply has no block to read.
	/// </summary>
	public BlockAddressStatus BlockAddressStatus { get; private set; }

	/// <summary>
	/// The address of the ddstats block; only meaningful while <see cref="BlockAddressStatus" /> is
	/// <see cref="BlockAddressStatus.Resolved" />.
	/// </summary>
	public long BlockAddress => _memoryBlockAddress;

	public void Initialize(long? ddstatsMarkerOffset)
	{
		_process = nativeMemoryService.GetDevilDaggersProcess();
		if (_process == null)
		{
			BlockAddressStatus = BlockAddressStatus.Unresolved;
			IsInitialized = false;
			return;
		}

		BlockAddressResult blockAddressResult = nativeMemoryService.ResolveBlockAddress(_process, ddstatsMarkerOffset);
		BlockAddressStatus = blockAddressResult.Status;
		IsInitialized = blockAddressResult.Status == BlockAddressStatus.Resolved;
		if (IsInitialized)
			_memoryBlockAddress = blockAddressResult.Address;
	}

	public void Scan()
	{
		if (_process == null)
			return;

		nativeMemoryService.ReadMemory(_process, _memoryBlockAddress, _mainBuffer, 0, MainBlock.Size);

		MainBlockPrevious = MainBlock;
		MainBlock = new MainBlock(_mainBuffer);
	}

	public byte[] GetStatsBuffer()
	{
		byte[] buffer = new byte[StatsBufferSize * MainBlock.StatsCount];
		GetStatsBuffer(buffer);
		return buffer;
	}

	public void GetStatsBuffer(byte[] buffer)
	{
		if (_process == null)
			throw new InvalidOperationException("Cannot get stats buffer while the process is unavailable.");

		nativeMemoryService.ReadMemory(_process, MainBlock.StatsBase, buffer, 0, buffer.Length);
	}

	public bool IsReplayValid()
	{
		if (_process == null || MainBlock.ReplayLength is <= 0 or > 30 * 1024 * 1024)
			return false;

		_replayIdentifierBuffer.AsSpan().Clear();
		nativeMemoryService.ReadMemory(_process, MainBlock.ReplayBase, _replayIdentifierBuffer, 0, _replayIdentifierBuffer.Length);
		return LocalReplayBinaryHeader.IdentifierIsValid(_replayIdentifierBuffer, out _);
	}

	public byte[] ReadReplayFromMemory()
	{
		if (_process == null)
			return [];

		byte[] buffer = new byte[MainBlock.ReplayLength];
		nativeMemoryService.ReadMemory(_process, MainBlock.ReplayBase, buffer, 0, buffer.Length);

		return buffer;
	}

	public void WriteReplayToMemory(byte[] replay)
	{
		if (_process == null)
			return;

		nativeMemoryService.WriteMemory(_process, MainBlock.ReplayBase, replay, 0, replay.Length);
		nativeMemoryService.WriteMemory(_process, _memoryBlockAddress + 312, BitConverter.GetBytes(replay.Length), 0, sizeof(int));
		nativeMemoryService.WriteMemory(_process, _memoryBlockAddress + 316, [1], 0, 1);
	}
}
