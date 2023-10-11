using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.Tools.EditorFileState;

public static class FileStates
{
	public const string UntitledName = "<untitled>";

	public static FileState<SpawnsetBinary, SpawnsetEditType> Spawnset { get; } = new(
		obj: SpawnsetBinary.CreateDefault(),
		defaultEditType: SpawnsetEditType.Reset,
		toBytes: s => s.ToBytes(),
		deepCopy: s => s.DeepCopy(),
		editTypeEquals: (a, b) => a == b);

	public static FileState<ReplayBinary<LocalReplayBinaryHeader>, ReplayEditType> Replay { get; } = new(
		obj: ReplayBinary<LocalReplayBinaryHeader>.CreateDefault(),
		defaultEditType: ReplayEditType.Reset,
		toBytes: r => r.Compile(),
		deepCopy: _ => throw new NotImplementedException(),
		editTypeEquals: (a, b) => a == b);
}
