using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class SpawnsetEditorMenu
{
	public static void Render()
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				RenderFileMenu();
				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Edit"))
			{
				RenderEditMenu();
				ImGui.EndMenu();
			}

			ImGui.EndMenuBar();
		}
	}

	private static void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewSpawnset();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenSpawnset();

		if (ImGui.MenuItem("Open default (V3)", "Ctrl+Shift+D"))
			OpenDefaultSpawnset();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			SaveSpawnset();

		if (ImGui.MenuItem("Save as", "Ctrl+Shift+S"))
			SaveSpawnsetAs();

		ImGui.Separator();

		if (ImGui.MenuItem("Open current", "Ctrl+Shift+O"))
			OpenCurrentSpawnset();

		if (ImGui.MenuItem("Replace current", "Ctrl+R"))
			ReplaceCurrentSpawnset();

		if (ImGui.MenuItem("Delete current", "Ctrl+D"))
			DeleteCurrentSpawnset();

		ImGui.Separator();

		if (ImGui.MenuItem("Close", "Esc"))
			Close();
	}

	private static void RenderEditMenu()
	{
		if (ImGui.MenuItem("Undo", "Ctrl+Z"))
			HistoryChild.Undo();

		if (ImGui.MenuItem("Redo", "Ctrl+Y"))
			HistoryChild.Redo();

		if (ImGui.MenuItem("Hardcode end loop") && FileStates.Spawnset.Object.HasEndLoop())
		{
			FileStates.Spawnset.Update(FileStates.Spawnset.Object.GetWithHardcodedEndLoop(20));
			SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnsTransformation);
		}

		if (ImGui.MenuItem("Trim start of spawns"))
		{
			FileStates.Spawnset.Update(FileStates.Spawnset.Object.GetWithTrimmedStart(100));
			SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnsTransformation);
		}
	}

	public static void NewSpawnset()
	{
		FileStates.Spawnset.PromptSave(() =>
		{
			FileStates.Spawnset.Update(SpawnsetBinary.CreateDefault());
			FileStates.Spawnset.SetFile(null, null);
			SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
			SpawnsChild.ClearAllSelections();
		});
	}

	public static void OpenSpawnset()
	{
		string? filePath = NativeFileDialog.CreateOpenFileDialog(null);
		if (filePath != null)
			FileStates.Spawnset.PromptSave(() => OpenSpawnset(filePath));
	}

	private static void OpenSpawnset(string filePath)
	{
		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			PopupManager.ShowError($"Could not open file '{filePath}'.");
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		if (SpawnsetBinary.TryParse(fileContents, out SpawnsetBinary? spawnsetBinary))
		{
			FileStates.Spawnset.Update(spawnsetBinary);
			FileStates.Spawnset.SetFile(filePath, Path.GetFileName(filePath));
		}
		else
		{
			PopupManager.ShowError($"The file '{filePath}' could not be parsed as a spawnset.");
			return;
		}

		SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
		SpawnsChild.ClearAllSelections();
	}

	public static void OpenDefaultSpawnset()
	{
		FileStates.Spawnset.PromptSave(() =>
		{
			FileStates.Spawnset.Update(ContentManager.Content.DefaultSpawnset.DeepCopy());
			FileStates.Spawnset.SetFile(null, "V3");
			SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
			SpawnsChild.ClearAllSelections();
		});
	}

	public static void SaveSpawnset()
	{
		if (FileStates.Spawnset.FilePath != null)
			FileStates.Spawnset.SaveFile(FileStates.Spawnset.FilePath);
		else
			SaveSpawnsetAs();
	}

	public static void SaveSpawnsetAs()
	{
		string? filePath = NativeFileDialog.CreateSaveFileDialog(null);
		if (filePath != null)
			FileStates.Spawnset.SaveFile(filePath);
	}

	public static void OpenCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			FileStates.Spawnset.PromptSave(() => OpenSpawnset(UserSettings.ModsSurvivalPath));
		else
			PopupManager.ShowError("There is no modded survival file to open.");
	}

	public static void ReplaceCurrentSpawnset()
	{
		File.WriteAllBytes(UserSettings.ModsSurvivalPath, FileStates.Spawnset.Object.ToBytes());
		PopupManager.ShowMessage("Successfully replaced current survival file", "The current survival file has been replaced with the current spawnset.");
	}

	public static void DeleteCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);

		PopupManager.ShowMessage("Successfully deleted current survival file", "The current survival file has been deleted.");
	}

	public static void Close()
	{
		FileStates.Spawnset.PromptSave(() => UiRenderer.Layout = LayoutType.Main);
	}
}
