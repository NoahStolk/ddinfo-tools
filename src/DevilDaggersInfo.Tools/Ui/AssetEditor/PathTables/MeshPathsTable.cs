using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class MeshPathsTable : IPathTable<MeshPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => DdMeshes.All.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
	}

	public static void RenderPath(int index)
	{
		MeshAssetInfo assetInfo = DdMeshes.All[index];
		MeshAssetPath? path = PathTableUtils.Find(FileStates.Mod.Object.Meshes, assetInfo.AssetName);

		PathTableUtils.RenderDefaultColumns(assetInfo);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##Mesh_{index}")))
			SetPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##Mesh_{index}")) && path != null)
			path.SetPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsolutePath ?? "<none>");
	}

	private static void SetPath(MeshAssetInfo assetInfo, MeshAssetPath? path)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null);
			FileStates.Mod.Object.Meshes.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
	}
}
