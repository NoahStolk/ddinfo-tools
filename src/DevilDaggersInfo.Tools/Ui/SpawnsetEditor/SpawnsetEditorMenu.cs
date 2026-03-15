using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

internal sealed class SpawnsetEditorMenu(
	FileStates fileStates,
	UiLayoutManager uiLayoutManager,
	NativeFileDialog nativeFileDialog,
	PopupManager popupManager,
	HistoryWindow historyWindow,
	SpawnsWindow spawnsWindow,
	SpawnsetSaver spawnsetSaver)
{
	public void Render()
	{
		if (ImGui.BeginMainMenuBar())
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

			ImGui.EndMainMenuBar();
		}
	}

	private void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewSpawnset();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenSpawnset();

		if (ImGui.MenuItem("Open default (V3)", "Ctrl+Shift+D"))
			OpenDefaultSpawnset();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			fileStates.SaveSpawnset();

		if (ImGui.MenuItem("Save as", "Ctrl+Shift+S"))
			fileStates.SaveSpawnsetAs();

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

	private void RenderEditMenu()
	{
		if (ImGui.MenuItem("Undo", "Ctrl+Z"))
			historyWindow.Undo();

		if (ImGui.MenuItem("Redo", "Ctrl+Y"))
			historyWindow.Redo();

		if (ImGui.MenuItem("Hardcode end loop") && fileStates.Spawnset.Object.HasEndLoop())
		{
			fileStates.Spawnset.Update(fileStates.Spawnset.Object.GetWithHardcodedEndLoop(20));
			spawnsetSaver.Save(SpawnsetEditType.SpawnsTransformation);
		}

		if (ImGui.MenuItem("Trim start of spawns"))
		{
			fileStates.Spawnset.Update(fileStates.Spawnset.Object.GetWithTrimmedStart(100));
			spawnsetSaver.Save(SpawnsetEditType.SpawnsTransformation);
		}
	}

	public void NewSpawnset()
	{
		fileStates.Spawnset.PromptSave(() =>
		{
			fileStates.Spawnset.Update(SpawnsetBinary.CreateDefault());
			fileStates.Spawnset.SetFile(null, null);
			spawnsetSaver.Save(SpawnsetEditType.Reset);
			spawnsWindow.ClearAllSelections();
		});
	}

	public void OpenSpawnset()
	{
		nativeFileDialog.CreateOpenFileDialog(OpenSpawnsetCallback, null);
	}

	private void OpenSpawnsetCallback(string? filePath)
	{
		if (filePath != null)
			fileStates.Spawnset.PromptSave(() => OpenSpawnset(filePath));
	}

	private void OpenSpawnset(string filePath)
	{
		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			popupManager.ShowError($"Could not open file '{filePath}'.", ex);
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		if (SpawnsetBinary.TryParse(fileContents, out SpawnsetBinary? spawnsetBinary))
		{
			fileStates.Spawnset.Update(spawnsetBinary);
			fileStates.Spawnset.SetFile(filePath, Path.GetFileName(filePath));
		}
		else
		{
			popupManager.ShowError($"The file '{filePath}' could not be parsed as a spawnset.");
			return;
		}

		spawnsetSaver.Save(SpawnsetEditType.Reset);
		spawnsWindow.ClearAllSelections();
	}

	public void OpenDefaultSpawnset()
	{
		fileStates.Spawnset.PromptSave(() =>
		{
			fileStates.Spawnset.Update(ContentManager.Content.DefaultSpawnset.DeepCopy());
			fileStates.Spawnset.SetFile(null, "V3");
			spawnsetSaver.Save(SpawnsetEditType.Reset);
			spawnsWindow.ClearAllSelections();
		});
	}

	public void OpenCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			fileStates.Spawnset.PromptSave(() => OpenSpawnset(UserSettings.ModsSurvivalPath));
		else
			popupManager.ShowError("There is no modded survival file to open.");
	}

	public void ReplaceCurrentSpawnset()
	{
		File.WriteAllBytes(UserSettings.ModsSurvivalPath, fileStates.Spawnset.Object.ToBytes());
		popupManager.ShowMessage("Successfully replaced current survival file", "The current survival file has been replaced with the current spawnset.");
	}

	public void DeleteCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);

		popupManager.ShowMessage("Successfully deleted current survival file", "The current survival file has been deleted.");
	}

	public void Close()
	{
		fileStates.Spawnset.PromptSave(() => uiLayoutManager.Layout = LayoutType.Main);
	}
}
