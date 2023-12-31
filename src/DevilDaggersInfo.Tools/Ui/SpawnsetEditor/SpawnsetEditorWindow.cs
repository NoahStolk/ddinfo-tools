using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public static class SpawnsetEditorWindow
{
	private static bool _isWindowFocusedPrevious;

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Spawnset Editor - {FileStates.Spawnset.FileName ?? FileStates.UntitledName}{(FileStates.Spawnset.IsModified && FileStates.Spawnset.FileName != null ? "*" : string.Empty)}###spawnset_editor"), ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			bool isWindowFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

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

		ImGui.End(); // End Spawnset Editor
	}
}
