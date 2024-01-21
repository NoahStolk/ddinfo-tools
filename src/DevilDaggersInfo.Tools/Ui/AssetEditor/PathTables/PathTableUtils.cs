using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

public static class PathTableUtils
{
	public static List<AudioAssetInfo> Audio { get; } = AudioAudio.All.ToList();
	public static List<MeshAssetInfo> Meshes { get; } = DdMeshes.All.ToList();
	public static List<ObjectBindingAssetInfo> ObjectBindings { get; } = DdObjectBindings.All.ToList();
	public static List<ShaderAssetInfo> Shaders { get; } = DdShaders.All.ToList();
	public static List<TextureAssetInfo> Textures { get; } = DdTextures.All.ToList();

	public static void SetupDefaultColumns()
	{
		ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160, 0);
		ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.WidthFixed, 80, 1);
	}

	public static void RenderDefaultColumns(AssetInfo assetInfo)
	{
		ImGui.TableNextColumn();
		ImGui.Text(assetInfo.AssetName);

		ImGui.TableNextColumn();
		if (assetInfo.IsProhibited)
			ImGui.TextColored(Color.Orange, "Prohibited");
		else
			ImGui.TextColored(Color.Green, "OK");
	}

	public static T? Find<T>(List<T> list, string name)
		where T : class, IAssetPath
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].AssetName == name)
				return list[i];
		}

		return null;
	}
}
