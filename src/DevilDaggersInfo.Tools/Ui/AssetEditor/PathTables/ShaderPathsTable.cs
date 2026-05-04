using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

internal sealed class ShaderPathsTable(FileStates fileStates, INativeFileDialog nativeFileDialog) : IPathTable
{
	public int ColumnCount => 4;

	public int PathCount => PathTableUtils.Shaders.Count;

	public void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Vertex Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
		ImGui.TableSetupColumn("Fragment Path", ImGuiTableColumnFlags.WidthStretch, 0, 3);
	}

	public void RenderPath(int index)
	{
		ShaderAssetInfo assetInfo = PathTableUtils.Shaders[index];
		ShaderAssetPath? path = PathTableUtils.Find(fileStates.Mod.Object.Shaders, assetInfo.AssetName);

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

	private void SetVertexPath(ShaderAssetInfo assetInfo, ShaderAssetPath? path)
	{
		if (path == null)
		{
			path = new ShaderAssetPath(assetInfo.AssetName, null, null);
			fileStates.Mod.Object.Shaders.Add(path);
		}

		nativeFileDialog.CreateOpenFileDialog(path.SetVertexPath, PathUtils.GetFileFilter(path.AssetType));
	}

	private void SetFragmentPath(ShaderAssetInfo assetInfo, ShaderAssetPath? path)
	{
		if (path == null)
		{
			path = new ShaderAssetPath(assetInfo.AssetName, null, null);
			fileStates.Mod.Object.Shaders.Add(path);
		}

		nativeFileDialog.CreateOpenFileDialog(path.SetFragmentPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public void Sort(uint sorting, bool sortAscending)
	{
		PathTableUtils.Shaders.Sort((a, b) =>
		{
			int result = sorting switch
			{
				0 => string.CompareOrdinal(a.AssetName, b.AssetName),
				1 => a.IsProhibited.CompareTo(b.IsProhibited),
				2 => string.CompareOrdinal(Find(a.AssetName)?.AbsoluteVertexPath ?? string.Empty, Find(b.AssetName)?.AbsoluteVertexPath ?? string.Empty),
				3 => string.CompareOrdinal(Find(a.AssetName)?.AbsoluteFragmentPath ?? string.Empty, Find(b.AssetName)?.AbsoluteFragmentPath ?? string.Empty),
				_ => throw new UnreachableException($"Invalid sorting index {sorting}."),
			};

			return sortAscending ? result : -result;
		});

		ShaderAssetPath? Find(string assetName) => fileStates.Mod.Object.Shaders.Find(audio => audio.AssetName == assetName);
	}
}
