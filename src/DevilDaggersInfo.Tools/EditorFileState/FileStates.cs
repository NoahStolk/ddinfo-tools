using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

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

	public static FileState<EditorReplayModel, ReplayEditType> Replay { get; } = new(
		obj: EditorReplayModel.CreateDefault(),
		defaultEditType: ReplayEditType.Reset,
		toBytes: r => r.ToBytes(),
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
