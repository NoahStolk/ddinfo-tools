using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
	private ProcessModule? _processModule;

	private readonly INativeMemoryService _nativeMemoryService;

	public GameMemoryService(INativeMemoryService nativeMemoryService)
	{
		_nativeMemoryService = nativeMemoryService;
	}

	public MainBlock MainBlockPrevious { get; private set; }
	public MainBlock MainBlock { get; private set; }

	public bool IsInitialized { get; private set; }

	public long DdstatsMarkerOffset { get; private set; }

	public long ProcessBaseAddress => _processModule == null ? 0 : _processModule.BaseAddress.ToInt64();

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
			_processModule = _process.MainModule;
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

	public void WriteExperimental<T>(long initialAddress, ReadOnlySpan<int> offsets, T value)
		where T : unmanaged
	{
		if (_process == null)
			return;

		Span<byte> buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
		_nativeMemoryService.WriteMemory(_process, FollowPointerChain(initialAddress, offsets), buffer, 0, buffer.Length);
	}

	public void ReadExperimental(long initialAddress, Span<byte> buffer, ReadOnlySpan<int> offsets)
	{
		Read(FollowPointerChain(initialAddress, offsets), buffer);
	}

	private long FollowPointerChain(long initialAddress, ReadOnlySpan<int> offsets)
	{
		long address = ReadPointer(ProcessBaseAddress + initialAddress);
		for (int i = 0; i < offsets.Length - 1; i++)
			address = ReadPointer(address + offsets[i]);

		return address + offsets[^1];
	}

	private long ReadPointer(long memoryAddress)
	{
		Span<byte> buffer = stackalloc byte[sizeof(long)];
		Read(memoryAddress, buffer);
		return BitConverter.ToInt64(buffer);
	}

	private void Read(long memoryAddress, Span<byte> buffer)
	{
		if (_process != null)
			_nativeMemoryService.ReadMemory(_process, memoryAddress, buffer, 0, buffer.Length);
	}
}
