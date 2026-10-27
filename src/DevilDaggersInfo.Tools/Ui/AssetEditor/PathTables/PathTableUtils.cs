using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;

internal static class PathTableUtils
{
	public static List<AudioAssetInfo> Audio { get; } = [.. AudioAudio.All];
	public static List<MeshAssetInfo> Meshes { get; } = [.. DdMeshes.All];
	public static List<ObjectBindingAssetInfo> ObjectBindings { get; } = [.. DdObjectBindings.All];
	public static List<ShaderAssetInfo> Shaders { get; } = [.. DdShaders.All];
	public static List<TextureAssetInfo> Textures { get; } = [.. DdTextures.All];

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
			ImGui.TextColored(new Vector4(1, 0.5f, 0, 1), "Prohibited");
		else
			ImGui.TextColored(new Vector4(0, 1, 0, 1), "OK");
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
