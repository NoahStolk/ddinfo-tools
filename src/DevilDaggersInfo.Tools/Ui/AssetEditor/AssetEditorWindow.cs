using DevilDaggersInfo.Tools.EditorFileState;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetEditorWindow
{
	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Asset Editor - {FileStates.Mod.FileName ?? FileStates.UntitledName}{(FileStates.Mod.IsModified && FileStates.Mod.FileName != null ? "*" : string.Empty)}###asset_editor"), ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			AssetEditorMenu.Render();
		}

		ImGui.End(); // End Asset Editor
	}
}
