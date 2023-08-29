using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SpawnsetEditorWindow
{
	private static bool _isWindowFocusedPrevious;

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
			ArenaChild.Render(isWindowFocused, !_isWindowFocusedPrevious && isWindowFocused);

			ImGui.SameLine();
			SettingsChild.Render();

			ImGui.SameLine();
			HistoryChild.Render();

			_isWindowFocusedPrevious = isWindowFocused;
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Spawnset Editor
	}
}
