using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModPreviewWindow
{
	private static ModBinaryToc? _modBinaryToc;
	private static long? _modFileSize;
	private static string? _selectedFileName;

	public static string? SelectedFileName
	{
		get => _selectedFileName;
		set
		{
			_selectedFileName = value;
			LoadChunks();
		}
	}

	public static void LoadChunks()
	{
		if (_selectedFileName == null)
			return;

		string filePath = Path.Combine(UserSettings.ModsDirectory, _selectedFileName);
		if (!File.Exists(filePath))
			return;

		try
		{
			using FileStream fs = new(filePath, FileMode.Open);
			_modFileSize = fs.Length;
			using BinaryReader reader = new(fs);
			_modBinaryToc = ModBinaryToc.FromReader(reader);
		}
		catch (InvalidModBinaryException)
		{
			_selectedFileName = null;
			_modBinaryToc = null;
			_modFileSize = null;
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error loading mod binary '{_selectedFileName}'.");
			PopupManager.ShowError($"Error loading mod binary '{_selectedFileName}'.\n\n" + ex.Message);
			_selectedFileName = null;
			_modBinaryToc = null;
			_modFileSize = null;
		}
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(640, 360));
		if (ImGui.Begin("Mod Preview", ImGuiWindowFlags.NoCollapse))
		{
			ImGui.PopStyleVar();

			if (_modBinaryToc == null || _selectedFileName == null)
			{
				ImGui.Text("Select a valid mod from the Mod Manager window to preview its contents.");
			}
			else
			{
				RenderFileInfoTable(_modBinaryToc);

				if (ImGui.Button("Toggle prohibited"))
					ModsDirectoryLogic.ToggleProhibitedAssets(_selectedFileName);

				RenderChunksTable(_modBinaryToc);
			}
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Mod preview
	}

	private static void RenderFileInfoTable(ModBinaryToc modBinaryToc)
	{
		if (ImGui.BeginTable("File info", 2, ImGuiTableFlags.Borders, new(512, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, 256);

			NextColumnText("File name");
			NextColumnText(_selectedFileName);

			NextColumnText("Binary type");
			NextColumnText(EnumUtils.ModBinaryTypeNames[modBinaryToc.Type]);

			NextColumnText("File size");
			NextColumnText(FileSizeUtils.Format(_modFileSize ?? 0));

			NextColumnText("Asset count");
			NextColumnText(Inline.Span(modBinaryToc.Chunks.Count));

			NextColumnText("Prohibited asset count");
			NextColumnText(Inline.Span(modBinaryToc.Chunks.Count(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name)))); // TODO: Cache.

			ImGui.EndTable();
		}
	}

	private static unsafe void RenderChunksTable(ModBinaryToc modBinaryToc)
	{
		if (ImGui.BeginTable("Chunks", 4, ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("Asset name", ImGuiTableColumnFlags.DefaultSort, 256, 0);
			ImGui.TableSetupColumn("Asset type", ImGuiTableColumnFlags.None, 128, 1);
			ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.None, 64, 2);
			ImGui.TableSetupColumn("Raw size", ImGuiTableColumnFlags.None, 64, 3);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				// ModsDirectoryLogic.SortModFiles(sorting, sortAscending);

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < modBinaryToc.Chunks.Count; i++)
			{
				ModBinaryChunk chunk = modBinaryToc.Chunks[i];

				ImGui.TableNextColumn();
				ImGui.Text(chunk.Name);

				ImGui.TableNextColumn();
				ImGui.TextColored(chunk.AssetType.GetColor(), EnumUtils.AssetTypeNames[chunk.AssetType]);

				ImGui.TableNextColumn();
				if (AssetContainer.GetIsProhibited(chunk.AssetType, chunk.Name))
					ImGui.TextColored(Color.Orange, "Prohibited");
				else
					ImGui.TextColored(Color.Green, "OK");

				ImGui.TableNextColumn();
				ColumnTextRight(FileSizeUtils.Format(chunk.Size));
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
