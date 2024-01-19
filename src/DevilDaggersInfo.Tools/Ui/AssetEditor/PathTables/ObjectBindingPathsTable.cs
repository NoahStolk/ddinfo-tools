using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class ObjectBindingPathsTable : IPathTable<ObjectBindingPathsTable>
{
	public static int ColumnCount => 2;

	public static int PathCount => FileStates.Mod.Object.ObjectBindings.Count;

	public static IAssetPath GetPath(int index)
	{
		return FileStates.Mod.Object.ObjectBindings[index];
	}

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
	}

	public static void RenderPath(int index)
	{
		AssetPath path = FileStates.Mod.Object.ObjectBindings[index];

		ImGui.TableNextColumn();
		ImGui.Text(path.AssetName);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ObjectBinding_{index}")))
			NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
		ImGui.SameLine();
		ImGui.Text(path.AbsolutePath ?? string.Empty);
	}

	public static void Add(AssetInfo assetInfo)
	{
		FileStates.Mod.Object.ObjectBindings.Add(new(assetInfo.AssetType, assetInfo.AssetName, null));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
		FileStates.Mod.Object.SortObjectBindings(sorting, sortAscending);
	}
}
