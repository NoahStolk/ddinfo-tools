using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.EditorFileState;

public static class FileStates
{
	public const string UntitledName = "<untitled>";

	public static FileState<SpawnsetBinary> Spawnset { get; } = new(SpawnsetBinary.CreateDefault(), s => s.ToBytes());

	public static FileState<ReplayBinary<LocalReplayBinaryHeader>> Replay { get; } = new(ReplayBinary<LocalReplayBinaryHeader>.CreateDefault(), r => r.Compile());
}
