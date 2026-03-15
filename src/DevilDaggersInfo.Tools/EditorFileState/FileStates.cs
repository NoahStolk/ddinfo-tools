using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools.EditorFileState;

internal sealed class FileStates
{
	public const string UntitledName = "<untitled>";

	private readonly NativeFileDialog _nativeFileDialog;

	public FileStates(PopupManager popupManager, NativeFileDialog nativeFileDialog)
	{
		_nativeFileDialog = nativeFileDialog;

		Spawnset = new FileState<SpawnsetBinary, SpawnsetEditType>(
			obj: SpawnsetBinary.CreateDefault(),
			defaultEditType: SpawnsetEditType.Reset,
			toHash: s => MD5.HashData(s.ToBytes()),
			save: s => s.ToBytes(),
			deepCopy: s => s.DeepCopy(),
			editTypeEquals: (a, b) => a == b,
			savePromptAction: action =>
			{
				popupManager.ShowQuestion(
					"Save spawnset?",
					"Do you want to save the current spawnset?",
					() =>
					{
						SaveSpawnset();
						action();
					},
					action);
			});

		Replay = new FileState<EditorReplayModel, ReplayEditType>(
			obj: EditorReplayModel.CreateDefault(),
			defaultEditType: ReplayEditType.Reset,
			toHash: r => r.ToHash(),
			save: r => r.ToLocalReplay().Compile(),
			deepCopy: _ => throw new NotImplementedException(),
			editTypeEquals: (a, b) => a == b,
			savePromptAction: _ => throw new NotImplementedException());

		Mod = new FileState<AssetPaths, AssetEditType>(
			obj: new AssetPaths(),
			defaultEditType: AssetEditType.Reset,
			toHash: m => MD5.HashData(m.ToJsonBytes()),
			save: m => m.ToJsonBytes(),
			deepCopy: _ => throw new NotImplementedException(),
			editTypeEquals: (a, b) => a == b,
			savePromptAction: _ => throw new NotImplementedException());
	}

	public FileState<SpawnsetBinary, SpawnsetEditType> Spawnset { get; }

	public FileState<EditorReplayModel, ReplayEditType> Replay { get; }

	public FileState<AssetPaths, AssetEditType> Mod { get; }

	public void SaveSpawnset()
	{
		if (Spawnset.FilePath != null)
			Spawnset.SaveFile(Spawnset.FilePath);
		else
			SaveSpawnsetAs();
	}

	public void SaveSpawnsetAs()
	{
		_nativeFileDialog.CreateSaveFileDialog(SaveSpawnsetAsCallback, null);
	}

	private void SaveSpawnsetAsCallback(string? filePath)
	{
		if (filePath != null)
			Spawnset.SaveFile(filePath);
	}
}
