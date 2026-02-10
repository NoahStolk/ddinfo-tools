using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public sealed class AssetEditorMenu
{
	private readonly UiLayoutManager _uiLayoutManager;

	public AssetEditorMenu(UiLayoutManager uiLayoutManager)
	{
		_uiLayoutManager = uiLayoutManager;
	}

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

	public static void NewMod()
	{
		FileStates.Mod.Update(new AssetPaths());
		FileStates.Mod.SetFile(null, null);
	}

	public static void OpenMod()
	{
		NativeFileDialog.CreateOpenFileDialog(OpenModCallback, PathUtils.FileExtensionMod);
	}

	private static void OpenModCallback(string? filePath)
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
			PopupManager.ShowError($"Could not open file '{filePath}'.", ex);
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
			PopupManager.ShowError($"File '{filePath}' could not be parsed as a mod.", ex);
			return;
		}

		if (mod == null)
		{
			PopupManager.ShowError($"File '{filePath}' could not be parsed as a mod.");
			return;
		}

		FileStates.Mod.Update(mod);
		FileStates.Mod.SetFile(filePath, Path.GetFileName(filePath));
	}

	public static void SaveMod()
	{
		if (FileStates.Mod.FilePath != null)
			FileStates.Mod.SaveFile(FileStates.Mod.FilePath);
		else
			SaveModAs();
	}

	public static void SaveModAs()
	{
		NativeFileDialog.CreateSaveFileDialog(SaveModCallback, PathUtils.FileExtensionMod);
	}

	private static void SaveModCallback(string? filePath)
	{
		if (filePath == null)
			return;

		filePath = Path.ChangeExtension(filePath, PathUtils.FileExtensionMod);
		FileStates.Mod.SaveFile(filePath);
	}

	public void Close()
	{
		FileStates.Mod.PromptSave(() => _uiLayoutManager.Layout = LayoutType.Main);
	}
}
