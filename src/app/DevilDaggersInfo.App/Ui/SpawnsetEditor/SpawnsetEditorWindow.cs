using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SpawnsetEditorWindow
{
	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		if (ImGui.Begin("Spawnset Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			bool isWindowFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

			ImGui.PopStyleVar();

			SpawnsetEditorMenu.Render();
			SpawnsChild.Render();

			ImGui.SameLine();
			ArenaChild.Render(isWindowFocused);

			ImGui.SameLine();
			SettingsChild.Render();

			ImGui.SameLine();
			HistoryChild.Render();
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Spawnset Editor
	}
}
