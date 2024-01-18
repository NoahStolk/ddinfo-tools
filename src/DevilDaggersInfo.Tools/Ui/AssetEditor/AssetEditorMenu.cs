using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;
using System.Security;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetEditorMenu
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

			ImGui.EndMenuBar();
		}
	}

	private static void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewMod();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenMod();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			SaveMod();

		ImGui.Separator();

		if (ImGui.MenuItem("Close", "Esc"))
			Close();
	}

	public static void NewMod()
	{
		FileStates.Mod.Update(new());
		FileStates.Mod.SetFile(null, null);
	}

	public static void OpenMod()
	{
		NativeFileDialog.CreateOpenFileDialog(OpenModCallback, "JSON files (*.json)|*.json");
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
		catch (Exception ex) when (ex is PathTooLongException or DirectoryNotFoundException or IOException or UnauthorizedAccessException or FileNotFoundException or NotSupportedException or SecurityException)
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
		NativeFileDialog.CreateSaveFileDialog(SaveModCallback, "JSON files (*.json)|*.json");
	}

	private static void SaveModCallback(string? filePath)
	{
		if (filePath == null)
			return;

		filePath = Path.ChangeExtension(filePath, ".json");
		FileStates.Mod.SaveFile(filePath);
	}

	public static void Close()
	{
		UiRenderer.Layout = LayoutType.Main;
	}
}
