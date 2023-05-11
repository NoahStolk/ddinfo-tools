using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SpawnsetEditorWindow
{
	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		ImGui.Begin("Spawnset Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse);
		ImGui.PopStyleVar();

		SpawnsetEditorMenu.Render();

		SpawnsChild.Render();

		ImGui.SameLine();
		ArenaChild.Render();

		ImGui.SameLine();
		SettingsChild.Render();

		ImGui.SameLine();
		HistoryChild.Render();

		ImGui.End();
	}
}
