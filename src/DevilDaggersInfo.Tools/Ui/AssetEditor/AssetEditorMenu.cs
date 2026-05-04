using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

internal sealed class AssetEditorMenu(UiLayoutManager uiLayoutManager, INativeFileDialog nativeFileDialog, PopupManager popupManager, FileStates fileStates)
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

			ImGui.EndMainMenuBar();
		}
	}

	private void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewMod();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenMod();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			SaveMod();

		if (ImGui.MenuItem("Save as", "Ctrl+Shift+S"))
			SaveModAs();

		ImGui.Separator();

		if (ImGui.MenuItem("Close", "Esc"))
			Close();
	}

	public void NewMod()
	{
		fileStates.Mod.Update(new AssetPaths());
		fileStates.Mod.SetFile(null, null);
	}

	public void OpenMod()
	{
		nativeFileDialog.CreateOpenFileDialog(OpenModCallback, PathUtils.FileExtensionMod);
	}

	private void OpenModCallback(string? filePath)
	{
		if (filePath == null)
			return;

		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			popupManager.ShowError($"Could not open file '{filePath}'.", ex);
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		AssetPaths? mod;
		try
		{
			mod = JsonSerializer.Deserialize(fileContents, AssetPathsContext.Default.AssetPaths);
		}
		catch (JsonException ex)
		{
			popupManager.ShowError($"File '{filePath}' could not be parsed as a mod.", ex);
			return;
		}

		if (mod == null)
		{
			popupManager.ShowError($"File '{filePath}' could not be parsed as a mod.");
			return;
		}

		fileStates.Mod.Update(mod);
		fileStates.Mod.SetFile(filePath, Path.GetFileName(filePath));
	}

	public void SaveMod()
	{
		if (fileStates.Mod.FilePath != null)
			fileStates.Mod.SaveFile(fileStates.Mod.FilePath);
		else
			SaveModAs();
	}

	public void SaveModAs()
	{
		nativeFileDialog.CreateSaveFileDialog(SaveModCallback, PathUtils.FileExtensionMod);
	}

	private void SaveModCallback(string? filePath)
	{
		if (filePath == null)
			return;

		filePath = Path.ChangeExtension(filePath, PathUtils.FileExtensionMod);
		fileStates.Mod.SaveFile(filePath);
	}

	public void Close()
	{
		fileStates.Mod.PromptSave(() => uiLayoutManager.Layout = LayoutType.Main);
	}
}
