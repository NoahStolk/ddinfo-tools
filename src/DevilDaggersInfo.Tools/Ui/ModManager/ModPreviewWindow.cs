using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

internal sealed class ModPreviewWindow(ModManagerState modManagerState, ModsDirectoryLogic modsDirectoryLogic)
{
	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(640, 360);
		if (ImGui.Begin("Mod Preview", ImGuiWindowFlags.NoCollapse))
		{
			if (modManagerState.SelectedFileName == null || modManagerState.BinaryType == null)
			{
				ImGui.Text("Select a valid mod from the Mod Manager window to preview its contents.");
			}
			else
			{
				RenderFileInfoTable(modManagerState.BinaryType.Value);

				if (ImGui.Button("Toggle prohibited"))
				{
					modsDirectoryLogic.ToggleAssets(modManagerState.SelectedFileName, toc =>
					{
						bool anyProhibited = toc.Entries.Any(c => AssetContainer.IsProhibited(c.AssetType, c.Name));
						return anyProhibited ? ModBinaryToc.DisableProhibitedAssets(toc) : ModBinaryToc.EnableAllAssets(toc);
					});
				}

				if (ImGui.Button("Toggle all"))
				{
					modsDirectoryLogic.ToggleAssets(modManagerState.SelectedFileName, toc =>
					{
						bool anyDisabled = toc.Entries.Any(c => !c.IsEnabled);
						return anyDisabled ? ModBinaryToc.EnableAllAssets(toc) : ModBinaryToc.DisableAllAssets(toc);
					});
				}

				RenderTocEntriesTable(modManagerState.SelectedFileName);
			}
		}

		ImGui.End();
	}

	private void RenderFileInfoTable(ModBinaryType modBinaryType)
	{
		if (ImGui.BeginTable("File info", 2, ImGuiTableFlags.Borders, new Vector2(512, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, 256);

			NextColumnText("File name");
			NextColumnText(modManagerState.SelectedFileName);

			NextColumnText("Binary type");
			NextColumnText(EnumUtils.ModBinaryTypeNames[modBinaryType]);

			NextColumnText("File size");
			NextColumnText(FileSizeUtils.Format(modManagerState.ModFileSize ?? 0));

			NextColumnText("Asset count");
			NextColumnText(Inline.Span(modManagerState.AssetCount));

			NextColumnText("Prohibited asset count");
			NextColumnText(Inline.Span(modManagerState.ProhibitedAssetCount));

			ImGui.EndTable();
		}
	}

	private unsafe void RenderTocEntriesTable(string selectedFileName)
	{
		if (ImGui.BeginTable("ModPreviewTocEntriesTable", 4, ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Asset name", ImGuiTableColumnFlags.DefaultSort, 256, 0);
			ImGui.TableSetupColumn("Asset type", ImGuiTableColumnFlags.None, 128, 1);
			ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.None, 72, 2);
			ImGui.TableSetupColumn("Raw size", ImGuiTableColumnFlags.None, 64, 3);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				modManagerState.DisplayedTocEntries = sorting switch
				{
					0 => sortAscending ? modManagerState.DisplayedTocEntries.OrderBy(c => c.Name.ToLower()).ToList() : modManagerState.DisplayedTocEntries.OrderByDescending(c => c.Name.ToLower()).ToList(),
					1 => sortAscending ? modManagerState.DisplayedTocEntries.OrderBy(c => c.AssetType).ToList() : modManagerState.DisplayedTocEntries.OrderByDescending(c => c.AssetType).ToList(),
					2 => sortAscending ? modManagerState.DisplayedTocEntries.OrderBy(c => AssetContainer.IsProhibited(c.AssetType, c.Name)).ToList() : modManagerState.DisplayedTocEntries.OrderByDescending(c => AssetContainer.IsProhibited(c.AssetType, c.Name)).ToList(),
					3 => sortAscending ? modManagerState.DisplayedTocEntries.OrderBy(c => c.Size).ToList() : modManagerState.DisplayedTocEntries.OrderByDescending(c => c.Size).ToList(),
					_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
				};

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < modManagerState.DisplayedTocEntries.Count; i++)
			{
				ModBinaryTocEntry tocEntry = modManagerState.DisplayedTocEntries[i];

				ImGui.TableNextColumn();
				if (ImGui.SmallButton(Inline.Span($"Toggle##{i}")))
				{
					modsDirectoryLogic.ToggleAssets(selectedFileName, toc =>
					{
						AssetKey key = new(tocEntry.AssetType, tocEntry.Name);
						return tocEntry.IsEnabled ? ModBinaryToc.DisableAsset(toc, key) : ModBinaryToc.EnableAsset(toc, key);
					});
				}

				ImGui.SameLine();
				ImGui.Text(tocEntry.Name);

				ImGui.TableNextColumn();
				ImGui.TextColored(tocEntry.AssetType.GetColor(), EnumUtils.AssetTypeNames[tocEntry.AssetType]);

				ImGui.TableNextColumn();
				if (AssetContainer.IsProhibited(tocEntry.AssetType, tocEntry.Name))
					ImGui.TextColored(Color.Orange, "Prohibited");
				else
					ImGui.TextColored(Color.Green, "OK");

				ImGui.TableNextColumn();
				ColumnTextRight(FileSizeUtils.Format(tocEntry.Size));
			}

			ImGui.EndTable();
		}
	}

	private static void ColumnTextRight(ReadOnlySpan<char> label)
	{
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(label).X - ImGui.GetScrollX());
		ImGui.Text(label);
	}

	private static void NextColumnText(ReadOnlySpan<char> label)
	{
		ImGui.TableNextColumn();
		ImGui.Text(label);
	}
}
