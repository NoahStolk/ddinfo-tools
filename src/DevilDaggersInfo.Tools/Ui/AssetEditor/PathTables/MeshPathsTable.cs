using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class MeshPathsTable : IPathTable<MeshPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => PathTableUtils.Meshes.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public static void RenderPath(int index)
	{
		MeshAssetInfo assetInfo = PathTableUtils.Meshes[index];
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
			path = new MeshAssetPath(assetInfo.AssetName, null);
			FileStates.Mod.Object.Meshes.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
		PathTableUtils.Meshes.Sort((a, b) =>
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

		static MeshAssetPath? Find(string assetName) => FileStates.Mod.Object.Meshes.Find(audio => audio.AssetName == assetName);
	}
}
