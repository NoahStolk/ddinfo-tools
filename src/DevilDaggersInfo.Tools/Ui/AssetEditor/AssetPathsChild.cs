using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;
using ImGuiNET;

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
				RenderAssets<AudioPathsTable>("Audio", _audioColor);
				RenderAssets<MeshPathsTable>("Meshes", _meshColor);
				RenderAssets<ObjectBindingPathsTable>("Object Bindings", _objectBindingColor);
				RenderAssets<ShaderPathsTable>("Shaders", _shaderColor);
				RenderAssets<TexturePathsTable>("Textures", _textureColor);

				ImGui.EndTabBar();
			}
		}

		ImGui.EndChild(); // End Asset Paths
	}

	private static unsafe void RenderAssets<T>(ReadOnlySpan<char> id, uint backgroundColor)
		where T : IPathTable<T>
	{
		const ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY;
		if (ImGui.BeginTabItem(Inline.Span($"{id}##Tab")))
		{
			if (ImGui.BeginTable(Inline.Span($"{id}_PathsTable"), T.ColumnCount, tableFlags))
			{
				ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
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

				ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
				clipper.Begin(T.PathCount);
				while (clipper.Step())
				{
					for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
					{
						ImGui.TableNextRow();
						ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, backgroundColor);

						T.RenderPath(i);
					}
				}

				ImGui.EndTable();
			}

			ImGui.EndTabItem();
		}
	}
}
