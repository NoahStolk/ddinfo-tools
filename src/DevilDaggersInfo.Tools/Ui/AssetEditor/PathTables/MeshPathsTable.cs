using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class MeshPathsTable : IPathTable<MeshPathsTable>
{
	public static int ColumnCount => 2;

	public static int PathCount => FileStates.Mod.Object.Meshes.Count;

	public static IAssetPath GetPath(int index)
	{
		return FileStates.Mod.Object.Meshes[index];
	}

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
	}

	public static void RenderPath(int index)
	{
		MeshAssetPath path = FileStates.Mod.Object.Meshes[index];

		ImGui.TableNextColumn();
		ImGui.Text(path.AssetName);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##Mesh_{index}")))
			NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
		ImGui.SameLine();
		ImGui.Text(path.AbsolutePath ?? string.Empty);
	}

	public static void Add(AssetInfo assetInfo)
	{
		FileStates.Mod.Object.Meshes.Add(new(assetInfo.AssetName, null));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
		FileStates.Mod.Object.SortMeshes(sorting, sortAscending);
	}
}
