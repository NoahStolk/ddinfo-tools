using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;

namespace DevilDaggersInfo.Tools.EditorFileState;

public static class FileStates
{
	public const string UntitledName = "<untitled>";

	public static FileState<SpawnsetBinary, SpawnsetEditType> Spawnset { get; } = new(
		obj: SpawnsetBinary.CreateDefault(),
		defaultEditType: SpawnsetEditType.Reset,
		toBytes: s => s.ToBytes(),
		deepCopy: s => s.DeepCopy(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: PopupManager.ShowSaveSpawnsetPrompt);

	public static FileState<ReplayBinary<LocalReplayBinaryHeader>, ReplayEditType> Replay { get; } = new(
		obj: ReplayBinary<LocalReplayBinaryHeader>.CreateDefault(),
		defaultEditType: ReplayEditType.Reset,
		toBytes: r => r.Compile(),
		deepCopy: _ => throw new NotImplementedException(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: _ => throw new NotImplementedException());

	public static FileState<AssetPaths, AssetEditType> Mod { get; } = new(
		obj: new(),
		defaultEditType: AssetEditType.Reset,
		toBytes: r => r.ToJsonBytes(),
		deepCopy: _ => throw new NotImplementedException(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: _ => throw new NotImplementedException());
}
