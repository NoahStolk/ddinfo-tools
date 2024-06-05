using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.EditorFileState;

public static class FileStates
{
	public const string UntitledName = "<untitled>";

	public static FileState<SpawnsetBinary, SpawnsetEditType> Spawnset { get; } = new(
		obj: SpawnsetBinary.CreateDefault(),
		defaultEditType: SpawnsetEditType.Reset,
		toHash: s => MD5.HashData(s.ToBytes()),
		save: s => s.ToBytes(),
		deepCopy: s => s.DeepCopy(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: PopupManager.ShowSaveSpawnsetPrompt);

	public static FileState<EditorReplayModel, ReplayEditType> Replay { get; } = new(
		obj: EditorReplayModel.CreateDefault(),
		defaultEditType: ReplayEditType.Reset,
		toHash: r => r.ToHash(),
		save: r => r.ToLocalReplay().Compile(),
		deepCopy: _ => throw new NotImplementedException(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: _ => throw new NotImplementedException());

	public static FileState<AssetPaths, AssetEditType> Mod { get; } = new(
		obj: new AssetPaths(),
		defaultEditType: AssetEditType.Reset,
		toHash: m => MD5.HashData(m.ToJsonBytes()),
		save: m => m.ToJsonBytes(),
		deepCopy: _ => throw new NotImplementedException(),
		editTypeEquals: (a, b) => a == b,
		savePromptAction: _ => throw new NotImplementedException());
}
