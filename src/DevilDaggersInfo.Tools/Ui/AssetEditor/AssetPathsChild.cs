using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class AssetPathsChild
{
	private static readonly uint _audioColor = GetColor(AssetType.Audio);
	private static readonly uint _meshColor = GetColor(AssetType.Mesh);
	private static readonly uint _objectBindingColor = GetColor(AssetType.ObjectBinding);
	private static readonly uint _shaderColor = GetColor(AssetType.Shader);
	private static readonly uint _textureColor = GetColor(AssetType.Texture);

	private static uint GetColor(AssetType assetType)
	{
		return ImGui.ColorConvertFloat4ToU32(assetType.GetColor() with { W = 0.1f });
	}

	public static void Render()
	{
		if (ImGui.BeginChild("Asset Paths"))
		{
			if (ImGui.BeginTabBar("Asset Browser Tabs"))
			{
				RenderAssets<AudioPathsTable>("Audio", AudioAudio.All, _audioColor);
				RenderAssets<MeshPathsTable>("Meshes", DdMeshes.All, _meshColor);
				RenderAssets<ObjectBindingPathsTable>("Object Bindings", DdObjectBindings.All, _objectBindingColor);
				RenderAssets<ShaderPathsTable>("Shaders", DdShaders.All, _shaderColor);
				RenderAssets<TexturePathsTable>("Textures", DdTextures.All, _textureColor);

				ImGui.EndTabBar();
			}
		}

		ImGui.EndChild(); // End Asset Paths
	}

	private static unsafe void RenderAssets<T>(ReadOnlySpan<char> id, IReadOnlyList<AssetInfo> assets, uint backgroundColor)
		where T : IPathTable<T>
	{
		if (ImGui.BeginTabItem(Inline.Span($"{id}##Tab")))
		{
			Vector2 size = ImGui.GetContentRegionAvail();
			size.Y /= 2;

			if (ImGui.BeginTable(Inline.Span($"{id}_PathsTable"), T.ColumnCount, ImGuiTableFlags.Borders | ImGuiTableFlags.Sortable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY, size))
			{
				T.SetupColumns();
				ImGui.TableHeadersRow();

				ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
				if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
				{
					uint sorting = sortsSpecs.Specs.ColumnUserID;
					bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

					T.Sort(sorting, sortAscending);

					sortsSpecs.SpecsDirty = false;
				}

				for (int i = 0; i < T.PathCount; i++)
				{
					ImGui.TableNextRow();
					ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, backgroundColor);

					T.RenderPath(i);
				}

				ImGui.EndTable();
			}

			ImGui.Text("Filter");

			RenderAvailableAssets<T>(id, assets);

			ImGui.EndTabItem();
		}
	}

	private static unsafe void RenderAvailableAssets<T>(ReadOnlySpan<char> id, IReadOnlyList<AssetInfo> assets)
		where T : IPathTable<T>
	{
		if (ImGui.BeginTable(Inline.Span($"{id}_AvailableAssetsTable"), 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
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
					bool presentInMod = IsPresentInMod<T>(assetInfo);
					ImGui.BeginDisabled(presentInMod);

					ImGui.TableNextRow();

					ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, presentInMod ? 0x00000000 : ImGui.ColorConvertFloat4ToU32(assetInfo.AssetType.GetColor() with { W = 0.1f }));

					ImGui.TableNextColumn();
					if (ImGui.Selectable(assetInfo.AssetName, false, ImGuiSelectableFlags.SpanAllColumns))
						T.Add(assetInfo);

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
	}

	private static bool IsPresentInMod<T>(AssetInfo assetInfo)
		where T : IPathTable<T>
	{
		for (int i = 0; i < T.PathCount; i++)
		{
			IAssetPath path = T.GetPath(i);
			if (path.AssetName == assetInfo.AssetName && path.AssetType == assetInfo.AssetType)
				return true;
		}

		return false;
	}
}
