using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.PathTables;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

internal sealed class AssetPathsChild(
	AudioPathsTable audioPathsTable,
	MeshPathsTable meshPathsTable,
	ObjectBindingPathsTable objectBindingPathsTable,
	ShaderPathsTable shaderPathsTable,
	TexturePathsTable texturePathsTable)
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

	public void Render()
	{
		if (ImGui.BeginChild("Asset Paths"))
		{
			if (ImGui.BeginTabBar("Asset Browser Tabs"))
			{
				RenderAssets(audioPathsTable, "Audio", _audioColor);
				RenderAssets(meshPathsTable, "Meshes", _meshColor);
				RenderAssets(objectBindingPathsTable, "Object Bindings", _objectBindingColor);
				RenderAssets(shaderPathsTable, "Shaders", _shaderColor);
				RenderAssets(texturePathsTable, "Textures", _textureColor);

				ImGui.EndTabBar();
			}
		}

		ImGui.EndChild();
	}

	private static unsafe void RenderAssets(IPathTable pathTable, ReadOnlySpan<char> id, uint backgroundColor)
	{
		const ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY;
		if (ImGui.BeginTabItem(Inline.Span($"{id}##Tab")))
		{
			if (ImGui.BeginTable(Inline.Span($"{id}_PathsTable"), pathTable.ColumnCount, tableFlags))
			{
				ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
				pathTable.SetupColumns();
				ImGui.TableHeadersRow();

				ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
				if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
				{
					uint sorting = sortsSpecs.Specs.ColumnUserID;
					bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

					pathTable.Sort(sorting, sortAscending);

					sortsSpecs.SpecsDirty = false;
				}

				ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
				clipper.Begin(pathTable.PathCount);
				while (clipper.Step())
				{
					for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
					{
						ImGui.TableNextRow();
						ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, backgroundColor);

						pathTable.RenderPath(i);
					}
				}

				clipper.End();

				ImGui.EndTable();
			}

			ImGui.EndTabItem();
		}
	}
}
