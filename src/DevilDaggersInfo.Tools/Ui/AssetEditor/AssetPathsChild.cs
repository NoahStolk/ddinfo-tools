using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetPathsChild
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Asset Paths", size))
		{
			if (ImGui.BeginTable("Asset Paths Table", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160);
				ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch);
				ImGui.TableHeadersRow();

				for (int i = 0; i < FileStates.Mod.Object.Paths.Count; i++)
				{
					AssetPath path = FileStates.Mod.Object.Paths[i];

					ImGui.TableNextRow();

					ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.ColorConvertFloat4ToU32(path.AssetType.GetColor() with { W = 0.1f }));

					ImGui.TableNextColumn();
					ImGui.Text(EnumUtils.AssetTypeNames[path.AssetType]);

					ImGui.TableNextColumn();
					ImGui.Text(path.AssetName);

					ImGui.TableNextColumn();
					ImGui.Text(path.AbsolutePath);
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild(); // End Asset Browser
	}
}
