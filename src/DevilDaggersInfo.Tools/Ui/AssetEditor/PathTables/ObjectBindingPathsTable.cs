using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class ObjectBindingPathsTable : IPathTable<ObjectBindingPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => PathTableUtils.ObjectBindings.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public static void RenderPath(int index)
	{
		ObjectBindingAssetInfo assetInfo = PathTableUtils.ObjectBindings[index];
		ObjectBindingAssetPath? path = PathTableUtils.Find(FileStates.Mod.Object.ObjectBindings, assetInfo.AssetName);

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
		PathTableUtils.ObjectBindings.Sort((a, b) =>
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

		static ObjectBindingAssetPath? Find(string assetName) => FileStates.Mod.Object.ObjectBindings.Find(audio => audio.AssetName == assetName);
	}
}
