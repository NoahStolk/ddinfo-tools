using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetBrowserChild
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Asset Browser", size))
		{
			ImGui.Text("These are all available assets in the game. Click on an asset to add it to the mod.");

			if (ImGui.BeginTabBar("Asset Browser Tabs"))
			{
				RenderAssets("audio/audio", AudioAudio.All);
				RenderAssets("dd/meshes", DdMeshes.All);
				RenderAssets("dd/object bindings", DdObjectBindings.All);
				RenderAssets("dd/shaders", DdShaders.All);
				RenderAssets("dd/textures", DdTextures.All);

				ImGui.EndTabBar();
			}
		}

		ImGui.EndChild(); // End Asset Browser
	}

	private static unsafe void RenderAssets(ReadOnlySpan<char> id, IReadOnlyList<AssetInfo> assets)
	{
		if (ImGui.BeginTabItem(id))
		{
			ImGui.Text("Filter");

			if (ImGui.BeginTable(Inline.Span($"Table{id}"), 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
			{
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 160);
				ImGui.TableSetupColumn("Prohibited for 1000+", ImGuiTableColumnFlags.WidthFixed, 160);
				ImGui.TableHeadersRow();

				ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
				clipper.Begin(assets.Count);
				while (clipper.Step())
				{
					for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
					{
						AssetInfo assetInfo = assets[i];

						bool presentInMod = IsPresentInMod(assetInfo);
						ImGui.BeginDisabled(presentInMod);

						ImGui.TableNextRow();

						ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, presentInMod ? 0x00000000 : ImGui.ColorConvertFloat4ToU32(assetInfo.AssetType.GetColor() with { W = 0.1f }));

						ImGui.TableNextColumn();
						if (ImGui.Selectable(assetInfo.AssetName, false, ImGuiSelectableFlags.SpanAllColumns))
							FileStates.Mod.Object.Paths.Add(new(assetInfo.AssetType, assetInfo.AssetName, string.Empty));

						ImGui.TableNextColumn();
						if (assetInfo.IsProhibited)
							ImGui.TextColored(Color.Orange, "Prohibited");
						else
							ImGui.TextColored(Color.Green, "OK");

						ImGui.EndDisabled();
					}
				}

				ImGui.EndTable();
			}

			ImGui.EndTabItem();
		}
	}

	private static bool IsPresentInMod(AssetInfo assetInfo)
	{
		for (int i = 0; i < FileStates.Mod.Object.Paths.Count; i++)
		{
			AssetPath path = FileStates.Mod.Object.Paths[i];
			if (path.AssetName == assetInfo.AssetName && path.AssetType == assetInfo.AssetType)
				return true;
		}

		return false;
	}
}
