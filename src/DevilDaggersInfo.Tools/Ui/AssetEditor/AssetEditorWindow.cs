using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetEditorWindow
{
	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin(Inline.Span($"Asset Editor - {FileStates.Mod.FileName ?? FileStates.UntitledName}{(FileStates.Mod.IsModified && FileStates.Mod.FileName != null ? "*" : string.Empty)}###asset_editor"), ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
		{
			AssetEditorMenu.Render();

			if (ImGui.BeginTable("Asset Editor Table", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 128);
				ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch);
				ImGui.TableHeadersRow();

				for (int i = 0; i < FileStates.Mod.Object.Paths.Count; i++)
				{
					AssetPath path = FileStates.Mod.Object.Paths[i];

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(EnumUtils.AssetTypeNames[path.AssetType]);

					ImGui.TableNextColumn();
					ImGui.Text(path.AssetName);

					ImGui.TableNextColumn();
					ImGui.Text(path.AbsolutePath);
				}

				ImGui.EndTable();

				// TODO: Add button to add new asset.
			}
		}

		ImGui.End(); // End Asset Editor
	}
}
