using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class ObjectBindingPathsTable : IPathTable<ObjectBindingPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => DdObjectBindings.All.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
	}

	public static void RenderPath(int index)
	{
		ObjectBindingAssetInfo assetInfo = DdObjectBindings.All[index];
		ObjectBindingAssetPath? path = FileStates.Mod.Object.ObjectBindings.Find(a => a.AssetName == assetInfo.AssetName);

		PathTableUtils.RenderDefaultColumns(assetInfo);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ObjectBinding_{index}")))
			SetPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##ObjectBinding_{index}")) && path != null)
			path.SetPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsolutePath ?? "<none>");
	}

	private static void SetPath(ObjectBindingAssetInfo assetInfo, ObjectBindingAssetPath? path)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null);
			FileStates.Mod.Object.ObjectBindings.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
	}
}
