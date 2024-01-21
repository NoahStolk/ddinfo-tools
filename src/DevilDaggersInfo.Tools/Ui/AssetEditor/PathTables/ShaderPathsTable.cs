using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class ShaderPathsTable : IPathTable<ShaderPathsTable>
{
	public static int ColumnCount => 4;

	public static int PathCount => DdShaders.All.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Vertex Path", ImGuiTableColumnFlags.WidthStretch, 0, 1);
		ImGui.TableSetupColumn("Fragment Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public static void RenderPath(int index)
	{
		ShaderAssetInfo assetInfo = DdShaders.All[index];
		ShaderAssetPath? path = FileStates.Mod.Object.Shaders.Find(a => a.AssetName == assetInfo.AssetName);

		PathTableUtils.RenderDefaultColumns(assetInfo);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ShaderVertex_{index}")))
			SetVertexPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##ShaderVertex_{index}")) && path != null)
			path.SetVertexPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsoluteVertexPath ?? "<none>");

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##ShaderFragment_{index}")))
			SetFragmentPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##ShaderFragment_{index}")) && path != null)
			path.SetFragmentPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsoluteFragmentPath ?? "<none>");
	}

	private static void SetVertexPath(ShaderAssetInfo assetInfo, ShaderAssetPath? path)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null, null);
			FileStates.Mod.Object.Shaders.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetVertexPath, PathUtils.GetFileFilter(path.AssetType));
	}

	private static void SetFragmentPath(ShaderAssetInfo assetInfo, ShaderAssetPath? path)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null, null);
			FileStates.Mod.Object.Shaders.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetFragmentPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
	}
}
