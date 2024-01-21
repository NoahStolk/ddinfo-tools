using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
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
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Prohibited for 1000+", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
	}

	public static void RenderPath(int index)
	{
		MeshAssetInfo assetInfo = DdMeshes.All[index];
		MeshAssetPath? path = FileStates.Mod.Object.Meshes.Find(a => a.AssetName == assetInfo.AssetName);

		ImGui.TableNextColumn();
		ImGui.Text(assetInfo.AssetName);

		ImGui.TableNextColumn();
		if (assetInfo.IsProhibited)
			ImGui.TextColored(Color.Orange, "Prohibited");
		else
			ImGui.TextColored(Color.Green, "OK");

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##Mesh_{index}")))
			SetPath(assetInfo, path);
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
