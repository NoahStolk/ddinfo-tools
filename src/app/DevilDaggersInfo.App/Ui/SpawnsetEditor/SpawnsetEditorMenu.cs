using DevilDaggersInfo.App.Ui.Popups;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

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

		if (ImGui.MenuItem("Hardcode end loop") && SpawnsetState.Spawnset.HasEndLoop())
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset.GetWithHardcodedEndLoop(20);
			SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnsTransformation);
		}

		if (ImGui.MenuItem("Trim start of spawns"))
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset.GetWithTrimmedStart(100);
			SpawnsetHistoryUtils.Save(SpawnsetEditType.SpawnsTransformation);
		}
	}

	public static void NewSpawnset()
	{
		SpawnsetState.PromptSaveSpawnset(() =>
		{
			SpawnsetState.Spawnset = SpawnsetBinary.CreateDefault();
			SpawnsetState.SetFile(null, null);
			SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
			SpawnsChild.ClearAllSelections();
		});
	}

	public static void OpenSpawnset()
	{
		string? filePath = NativeFileDialog.CreateOpenFileDialog(null);
		if (filePath != null)
			SpawnsetState.PromptSaveSpawnset(() => OpenSpawnset(filePath));
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
			SpawnsetState.Spawnset = spawnsetBinary;
			SpawnsetState.SetFile(filePath, Path.GetFileName(filePath));
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
		SpawnsetState.PromptSaveSpawnset(() =>
		{
			SpawnsetState.Spawnset = ContentManager.Content.DefaultSpawnset.DeepCopy();
			SpawnsetState.SetFile(null, "V3");
			SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
			SpawnsChild.ClearAllSelections();
		});
	}

	public static void SaveSpawnset()
	{
		if (SpawnsetState.SpawnsetPath != null)
			SpawnsetState.SaveFile(SpawnsetState.SpawnsetPath);
		else
			SaveSpawnsetAs();
	}

	public static void SaveSpawnsetAs()
	{
		string? filePath = NativeFileDialog.CreateSaveFileDialog(null);
		if (filePath != null)
			SpawnsetState.SaveFile(filePath);
	}

	public static void OpenCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			SpawnsetState.PromptSaveSpawnset(() => OpenSpawnset(UserSettings.ModsSurvivalPath));
		else
			PopupManager.ShowError("There is no modded survival file to open.");
	}

	public static void ReplaceCurrentSpawnset()
	{
		File.WriteAllBytes(UserSettings.ModsSurvivalPath, SpawnsetState.Spawnset.ToBytes());
		PopupManager.ShowReplacedSurvivalFile();
	}

	public static void DeleteCurrentSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);

		PopupManager.ShowDeletedSurvivalFile();
	}

	public static void Close()
	{
		SpawnsetState.PromptSaveSpawnset(() => UiRenderer.Layout = LayoutType.Main);
	}
}
