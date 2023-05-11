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

			ImGui.EndMenuBar();
		}
	}

	private static void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewSpawnset();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenSpawnset();

		if (ImGui.MenuItem("Open default (V3)"))
			OpenDefaultSpawnset();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			SaveSpawnset();

		if (ImGui.MenuItem("Replace", "Ctrl+R"))
			ReplaceSpawnset();

		ImGui.Separator();

		if (ImGui.MenuItem("Close", "Esc"))
			Close();
	}

	public static void NewSpawnset()
	{
		SpawnsetState.SpawnsetName = "(untitled)";
		SpawnsetState.Spawnset = SpawnsetBinary.CreateDefault();
		SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
	}

	public static void OpenSpawnset()
	{
		string? filePath = NativeFileDialog.CreateOpenFileDialog(null);
		if (filePath == null)
			return;

		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			Modals.ShowError($"Could not open file '{filePath}'.");
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		if (SpawnsetBinary.TryParse(fileContents, out SpawnsetBinary? spawnsetBinary))
		{
			SpawnsetState.SpawnsetName = Path.GetFileName(filePath);
			SpawnsetState.Spawnset = spawnsetBinary;
		}
		else
		{
			Modals.ShowError($"The file '{filePath}' could not be parsed as a spawnset.");
			return;
		}

		SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
	}

	private static void OpenDefaultSpawnset()
	{
		SpawnsetState.SpawnsetName = "V3";
		SpawnsetState.Spawnset = ContentManager.Content.DefaultSpawnset.DeepCopy();
		SpawnsetHistoryUtils.Save(SpawnsetEditType.Reset);
	}

	public static void SaveSpawnset()
	{
		string? filePath = NativeFileDialog.CreateSaveFileDialog(null);
		if (filePath != null)
			File.WriteAllBytes(filePath, SpawnsetState.Spawnset.ToBytes());
	}

	public static void ReplaceSpawnset()
	{
		File.WriteAllBytes(UserSettings.ModsSurvivalPath, SpawnsetState.Spawnset.ToBytes());
		Modals.ShowReplacedSurvivalFile();
	}

	public static void Close()
	{
		UiRenderer.Layout = LayoutType.Main;
	}
}
