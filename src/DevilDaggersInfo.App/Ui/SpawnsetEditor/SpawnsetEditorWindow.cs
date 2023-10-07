using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SpawnsetEditorWindow
{
	private static bool _isWindowFocusedPrevious;

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Spawnset Editor - {SpawnsetState.SpawnsetName ?? SpawnsetState.UntitledName}{(SpawnsetState.IsSpawnsetModified && SpawnsetState.SpawnsetName != null ? "*" : string.Empty)}###spawnset_editor"), ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			bool isWindowFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

			ImGui.PopStyleVar();

			SpawnsetEditorMenu.Render();
			SpawnsChild.Render();

			ImGui.SameLine();
			ArenaChild.Render(isWindowFocused, !_isWindowFocusedPrevious && isWindowFocused);

			ImGui.SameLine();

			if (ImGui.BeginChild("SettingsAndHistoryChild"))
			{
				SettingsChild.Render();

				ImGui.SameLine();
				HistoryChild.Render();

				SpawnsetWarningsChild.Render();
			}

			ImGui.EndChild(); // End SettingsAndHistoryChild

			_isWindowFocusedPrevious = isWindowFocused;
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Spawnset Editor
	}
}
