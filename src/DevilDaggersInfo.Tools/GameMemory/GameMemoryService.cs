using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.GameMemory;

public class GameMemoryService
{
	public const int MainBufferSize = 319;
	public const int StatsBufferSize = 112;

	private readonly byte[] _pointerBuffer = new byte[sizeof(long)];
	private readonly byte[] _mainBuffer = new byte[MainBufferSize];
	private readonly byte[] _replayIdentifierBuffer = new byte[LocalReplayBinaryHeader.IdentifierLength];

	private long _memoryBlockAddress;
	private Process? _process;

	private readonly INativeMemoryService _nativeMemoryService;

	public GameMemoryService(INativeMemoryService nativeMemoryService)
	{
		_nativeMemoryService = nativeMemoryService;
	}

	public MainBlock MainBlockPrevious { get; private set; }
	public MainBlock MainBlock { get; private set; }

	public bool IsInitialized { get; private set; }

	public long DdstatsMarkerOffset { get; private set; }

	public long ProcessBaseAddress => _process?.MainModule == null ? 0 : _process.MainModule.BaseAddress.ToInt64();

	public void Initialize(long ddstatsMarkerOffset)
	{
		DdstatsMarkerOffset = ddstatsMarkerOffset;

		_process = _nativeMemoryService.GetDevilDaggersProcess();
		if (_process?.MainModule == null)
		{
			IsInitialized = false;
		}
		else
		{
			_pointerBuffer.AsSpan().Clear();
			_nativeMemoryService.ReadMemory(_process, ProcessBaseAddress + ddstatsMarkerOffset, _pointerBuffer, 0, sizeof(long));
			_memoryBlockAddress = BitConverter.ToInt64(_pointerBuffer);
			IsInitialized = true;
		}
	}

	public void Scan()
	{
		if (_process == null)
			return;

		_nativeMemoryService.ReadMemory(_process, _memoryBlockAddress, _mainBuffer, 0, MainBufferSize);

		MainBlockPrevious = MainBlock;
		MainBlock = new(_mainBuffer);
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

		_nativeMemoryService.ReadMemory(_process, MainBlock.StatsBase, buffer, 0, buffer.Length);
	}

	public bool IsReplayValid()
	{
		if (_process == null || MainBlock.ReplayLength is <= 0 or > 30 * 1024 * 1024)
			return false;

		_replayIdentifierBuffer.AsSpan().Clear();
		_nativeMemoryService.ReadMemory(_process, MainBlock.ReplayBase, _replayIdentifierBuffer, 0, _replayIdentifierBuffer.Length);
		return LocalReplayBinaryHeader.IdentifierIsValid(_replayIdentifierBuffer, out _);
	}

	public byte[] ReadReplayFromMemory()
	{
		if (_process == null)
			return Array.Empty<byte>();

		byte[] buffer = new byte[MainBlock.ReplayLength];
		_nativeMemoryService.ReadMemory(_process, MainBlock.ReplayBase, buffer, 0, buffer.Length);

		return buffer;
	}

	public void WriteReplayToMemory(byte[] replay)
	{
		if (_process == null)
			return;

		_nativeMemoryService.WriteMemory(_process, MainBlock.ReplayBase, replay, 0, replay.Length);
		_nativeMemoryService.WriteMemory(_process, _memoryBlockAddress + 312, BitConverter.GetBytes(replay.Length), 0, sizeof(int));
		_nativeMemoryService.WriteMemory(_process, _memoryBlockAddress + 316, [1], 0, 1);
	}
}
