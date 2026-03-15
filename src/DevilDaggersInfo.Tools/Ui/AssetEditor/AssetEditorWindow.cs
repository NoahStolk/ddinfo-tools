using DevilDaggersInfo.Tools.EditorFileState;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

internal sealed class AssetEditorWindow(AssetPathsChild assetPathsChild, FileStates fileStates)
{
	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Asset Editor - {fileStates.Mod.FileName ?? FileStates.UntitledName}{(fileStates.Mod.IsModified && fileStates.Mod.FileName != null ? "*" : string.Empty)}###asset_editor"), ImGuiWindowFlags.NoCollapse))
		{
			assetPathsChild.Render();
		}

		ImGui.End();
	}
}
