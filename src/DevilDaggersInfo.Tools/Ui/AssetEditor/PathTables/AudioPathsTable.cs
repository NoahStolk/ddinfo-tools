using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class AudioPathsTable : IPathTable<AudioPathsTable>
{
	public static int ColumnCount => 3;

	public static int PathCount => FileStates.Mod.Object.Audio.Count;

	public static IAssetPath GetPath(int index)
	{
		return FileStates.Mod.Object.Audio[index];
	}

	public static void SetupColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 1);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
		ImGui.TableSetupColumn("Loudness", ImGuiTableColumnFlags.WidthFixed, 160, 3);
	}

	public static void RenderPath(int index)
	{
		AudioAssetPath path = FileStates.Mod.Object.Audio[index];

		ImGui.TableNextColumn();
		ImGui.Text(path.AssetName);

		ImGui.TableNextColumn();

		if (ImGui.Button(Inline.Span($"Browse##Audio_{index}")))
			NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));

		ImGui.SameLine();
		ImGui.Text(path.AbsolutePath ?? string.Empty);

		ImGui.TableNextColumn();
		float loudness = path.Loudness ?? 0f;
		if (ImGui.InputFloat(Inline.Span($"##Loudness_{index}"), ref loudness, 0.1f, 1f, "%.1f"))
			path.SetLoudness(loudness);
	}

	public static void Add(AssetInfo assetInfo)
	{
		FileStates.Mod.Object.Audio.Add(new(assetInfo.AssetType, assetInfo.AssetName, null, null));
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
		FileStates.Mod.Object.SortAudio(sorting, sortAscending);
	}
}
