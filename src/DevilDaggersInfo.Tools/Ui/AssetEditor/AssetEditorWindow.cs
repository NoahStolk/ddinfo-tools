using DevilDaggersInfo.Tools.EditorFileState;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetEditorWindow
{
	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Asset Editor - {FileStates.Mod.FileName ?? FileStates.UntitledName}{(FileStates.Mod.IsModified && FileStates.Mod.FileName != null ? "*" : string.Empty)}###asset_editor"), ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
		{
			AssetEditorMenu.Render();

			Vector2 size = ImGui.GetContentRegionAvail();
			size.Y /= 2;
			size.Y -= 4;
			AssetPathsChild.Render(size);
			ImGui.Separator();
			AssetBrowserChild.Render(size);
		}

		ImGui.End(); // End Asset Editor
	}
}
