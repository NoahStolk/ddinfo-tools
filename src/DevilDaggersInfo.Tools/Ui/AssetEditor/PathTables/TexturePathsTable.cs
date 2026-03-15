using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

internal sealed class TexturePathsTable(FileStates fileStates, INativeFileDialog nativeFileDialog) : IPathTable
{
	public int ColumnCount => 3;

	public int PathCount => PathTableUtils.Textures.Count;

	public void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public void RenderPath(int index)
	{
		TextureAssetInfo assetInfo = PathTableUtils.Textures[index];
		TextureAssetPath? path = PathTableUtils.Find(fileStates.Mod.Object.Textures, assetInfo.AssetName);

		PathTableUtils.RenderDefaultColumns(assetInfo);

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##Texture_{index}")))
			SetPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##Texture_{index}")) && path != null)
			path.SetPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsolutePath ?? "<none>");
	}

	private void SetPath(TextureAssetInfo assetInfo, TextureAssetPath? path)
	{
		if (path == null)
		{
			path = new TextureAssetPath(assetInfo.AssetName, null);
			fileStates.Mod.Object.Textures.Add(path);
		}

		nativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public void Sort(uint sorting, bool sortAscending)
	{
		PathTableUtils.Textures.Sort((a, b) =>
		{
			int result = sorting switch
			{
				0 => string.CompareOrdinal(a.AssetName, b.AssetName),
				1 => a.IsProhibited.CompareTo(b.IsProhibited),
				2 => string.CompareOrdinal(Find(a.AssetName)?.AbsolutePath ?? string.Empty, Find(b.AssetName)?.AbsolutePath ?? string.Empty),
				_ => throw new UnreachableException($"Invalid sorting index {sorting}."),
			};

			return sortAscending ? result : -result;
		});

		TextureAssetPath? Find(string assetName) => fileStates.Mod.Object.Textures.Find(audio => audio.AssetName == assetName);
	}
}
