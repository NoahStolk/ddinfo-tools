using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class ShaderPathsTable : IPathTable<ShaderPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => FileStates.Mod.Object.Shaders.Count;

	public static IAssetPath GetPath(int index)
	{
		return FileStates.Mod.Object.Shaders[index];
	}

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Vertex Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
		ImGui.TableSetupColumn("Fragment Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public static void RenderPath(int index)
	{
		ShaderAssetPath path = FileStates.Mod.Object.Shaders[index];

		ImGui.TableNextColumn();
		ImGui.Text(path.AssetName);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ShaderVertex_{index}")))
			NativeFileDialog.CreateOpenFileDialog(path.SetVertexPath, PathUtils.GetFileFilter(path.AssetType));
		ImGui.SameLine();
		ImGui.Text(path.AbsoluteVertexPath ?? string.Empty);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ShaderFragment_{index}")))
			NativeFileDialog.CreateOpenFileDialog(path.SetFragmentPath, PathUtils.GetFileFilter(path.AssetType));
		ImGui.SameLine();
		ImGui.Text(path.AbsoluteFragmentPath ?? string.Empty);
	}

	public static void Add(AssetInfo assetInfo)
	{
		FileStates.Mod.Object.Shaders.Add(new(assetInfo.AssetName, null, null));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
		FileStates.Mod.Object.SortShaders(sorting, sortAscending);
	}
}
