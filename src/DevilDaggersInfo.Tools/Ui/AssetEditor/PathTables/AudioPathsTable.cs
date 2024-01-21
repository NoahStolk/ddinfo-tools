using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public sealed class AudioPathsTable : IPathTable<AudioPathsTable>
{
	public static int ColumnCount => 4;

	public static int PathCount => AudioAudio.All.Count;

	public static void SetupColumns()
	{
		PathTableUtils.SetupDefaultColumns();
		ImGui.TableSetupColumn("Loudness", ImGuiTableColumnFlags.WidthFixed, 160, 1);
		ImGui.TableSetupColumn("Path", ImGuiTableColumnFlags.WidthStretch, 0, 2);
	}

	public static void RenderPath(int index)
	{
		AudioAssetInfo assetInfo = AudioAudio.All[index];
		AudioAssetPath? path = PathTableUtils.Find(FileStates.Mod.Object.Audio, assetInfo.AssetName);

		PathTableUtils.RenderDefaultColumns(assetInfo);

		ImGui.TableNextColumn();
		bool hasLoudness = path?.Loudness.HasValue ?? false;
		if (ImGui.Checkbox(Inline.Span($"##CheckboxLoudness_{index}"), ref hasLoudness))
			SetLoudness(assetInfo, path, hasLoudness ? assetInfo.DefaultLoudness : null);
		if (ImGui.IsItemHovered())
			ImGui.SetTooltip("If checked, the loudness will be overridden by the specified value.");
		ImGui.SameLine();
		ImGui.BeginDisabled(!hasLoudness);
		float loudness = path?.Loudness ?? assetInfo.DefaultLoudness;
		if (ImGui.InputFloat(Inline.Span($"##Loudness_{index}"), ref loudness, 0.1f, 1f, "%.1f"))
			SetLoudness(assetInfo, path, loudness);
		ImGui.EndDisabled();

		ImGui.TableNextColumn();
		if (ImGui.Button(Inline.Span($"Browse##Audio_{index}")))
			SetPath(assetInfo, path);
		ImGui.SameLine();
		if (ImGui.Button(Inline.Span($"Clear##Audio_{index}")) && path != null)
			path.SetPath(null);
		ImGui.SameLine();
		ImGui.Text(path?.AbsolutePath ?? "<none>");
	}

	private static void SetPath(AudioAssetInfo assetInfo, AudioAssetPath? path)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null, null);
			FileStates.Mod.Object.Audio.Add(path);
		}

		NativeFileDialog.CreateOpenFileDialog(path.SetPath, PathUtils.GetFileFilter(path.AssetType));
	}

	private static void SetLoudness(AudioAssetInfo assetInfo, AudioAssetPath? path, float? loudness)
	{
		if (path == null)
		{
			path = new(assetInfo.AssetName, null, null);
			FileStates.Mod.Object.Audio.Add(path);
		}

		path.SetLoudness(loudness);
	}

	public static void Sort(uint sorting, bool sortAscending)
	{
	}
}
