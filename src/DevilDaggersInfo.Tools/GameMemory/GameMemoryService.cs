using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using DevilDaggersInfo.Web.ApiSpec.Tools;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.GameMemory;

internal sealed class GameMemoryService(INativeMemoryService nativeMemoryService)
{
	public const int StatsBufferSize = 112;

	private const int _mainBufferSize = 319;

	private readonly byte[] _pointerBuffer = new byte[sizeof(long)];
	private readonly byte[] _mainBuffer = new byte[_mainBufferSize];
	private readonly byte[] _replayIdentifierBuffer = new byte[LocalReplayBinaryHeader.IdentifierLength];

	private long _memoryBlockAddress;
	private Process? _process;

	public MainBlock MainBlockPrevious { get; private set; }
	public MainBlock MainBlock { get; private set; }

	public bool IsInitialized { get; private set; }

	/// <summary>
	/// The build of the game the current process is running, or <see langword="null"/> when no process was found. Determines which marker to pass to <see cref="Initialize"/>.
	/// </summary>
	public AppOperatingSystem? GameOperatingSystem { get; private set; }

	public void FindProcess()
	{
		_process = nativeMemoryService.GetDevilDaggersProcess();
		GameOperatingSystem = _process == null ? null : nativeMemoryService.GetGameOperatingSystem(_process);
	}

	/// <summary>
	/// Initializes the process found by the last call to <see cref="FindProcess"/>.
	/// </summary>
	public void Initialize(long ddstatsMarkerOffset)
	{
		long? baseAddress = _process == null ? null : nativeMemoryService.GetGameBaseAddress(_process);
		if (_process == null || !baseAddress.HasValue)
		{
			IsInitialized = false;
		}
		else
		{
			_pointerBuffer.AsSpan().Clear();
			nativeMemoryService.ReadMemory(_process, baseAddress.Value + ddstatsMarkerOffset, _pointerBuffer, 0, sizeof(long));
			_memoryBlockAddress = BitConverter.ToInt64(_pointerBuffer);
			IsInitialized = true;
		}
	}

	public void Scan()
	{
		if (_process == null)
			return;

		nativeMemoryService.ReadMemory(_process, _memoryBlockAddress, _mainBuffer, 0, _mainBufferSize);

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
