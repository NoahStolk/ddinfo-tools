using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetBrowserWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Asset Browser"))
		{
			if (ImGui.BeginTable("Asset Browser Table", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 128);
				ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableHeadersRow();

				RenderAssets(AudioAudio.All);
				RenderAssets(DdMeshes.All);
				RenderAssets(DdObjectBindings.All);
				RenderAssets(DdShaders.All);
				RenderAssets(DdTextures.All);

				ImGui.EndTable();
			}
		}

		ImGui.End(); // End Asset Browser
	}

	private static unsafe void RenderAssets(IReadOnlyList<AssetInfo> assets)
	{
		ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
		clipper.Begin(assets.Count);
		while (clipper.Step())
		{
			for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
			{
				AssetInfo assetInfo = assets[i];

				ImGui.TableNextRow();

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.ColorConvertFloat4ToU32(assetInfo.AssetType.GetColor() with { W = 0.1f }));

				ImGui.TableNextColumn();
				ImGui.Text(EnumUtils.AssetTypeNames[assetInfo.AssetType]);

				ImGui.TableNextColumn();
				ImGui.Text(assetInfo.AssetName);

				ImGui.TableNextColumn();
				ImGui.Text(assetInfo.IsProhibited ? "Yes" : "No");
			}
		}
	}
}
