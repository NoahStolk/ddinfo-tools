using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModPreviewWindow
{
	private static List<ModBinaryTocEntry> _displayedTocEntries = [];
	private static ModBinaryType? _binaryType;
	private static int _assetCount;
	private static int _prohibitedAssetCount;
	private static long? _modFileSize;
	private static string? _selectedFileName;

	public static string? SelectedFileName
	{
		get => _selectedFileName;
		set
		{
			_selectedFileName = value;
			LoadTocEntries();
		}
	}

	public static void LoadTocEntries()
	{
		if (_selectedFileName == null)
			return;

		string filePath = Path.Combine(UserSettings.ModsDirectory, _selectedFileName);
		if (!File.Exists(filePath))
			return;

		try
		{
			using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
			_modFileSize = fs.Length;
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);
			_binaryType = modBinaryToc.Type;
			_assetCount = modBinaryToc.Entries.Count;
			_prohibitedAssetCount = modBinaryToc.Entries.Count(c => AssetContainer.IsProhibited(c.AssetType, c.Name));
			_displayedTocEntries.Clear();
			_displayedTocEntries.AddRange(modBinaryToc.Entries);
		}
		catch (InvalidModBinaryException)
		{
			ClearState();
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error loading mod binary '{_selectedFileName}'.");
			PopupManager.ShowError($"Error loading mod binary '{_selectedFileName}'.", ex);
			ClearState();
		}
	}

	public static void DeleteIfSelected(string fileName)
	{
		if (SelectedFileName == fileName)
			SelectedFileName = null;
	}

	public static void UpdateIfSelected(string oldFileName, string newFileName)
	{
		if (SelectedFileName == oldFileName)
			SelectedFileName = newFileName;
	}

	private static void ClearState()
	{
		_displayedTocEntries.Clear();
		_binaryType = null;
		_assetCount = 0;
		_prohibitedAssetCount = 0;
		_selectedFileName = null;
		_modFileSize = null;
	}

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(640, 360);
		if (ImGui.Begin("Mod Preview", ImGuiWindowFlags.NoCollapse))
		{
			if (_selectedFileName == null || _binaryType == null)
			{
				ImGui.Text("Select a valid mod from the Mod Manager window to preview its contents.");
			}
			else
			{
				RenderFileInfoTable(_binaryType.Value);

				if (ImGui.Button("Toggle prohibited"))
					ModsDirectoryLogic.ToggleProhibitedAssets(_selectedFileName);

				RenderTocEntriesTable();
			}
		}

		ImGui.End(); // End Mod preview
	}

	private static void RenderFileInfoTable(ModBinaryType modBinaryType)
	{
		if (ImGui.BeginTable("File info", 2, ImGuiTableFlags.Borders, new(512, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, 256);

			NextColumnText("File name");
			NextColumnText(_selectedFileName);

			NextColumnText("Binary type");
			NextColumnText(EnumUtils.ModBinaryTypeNames[modBinaryType]);

			NextColumnText("File size");
			NextColumnText(FileSizeUtils.Format(_modFileSize ?? 0));

			NextColumnText("Asset count");
			NextColumnText(Inline.Span(_assetCount));

			NextColumnText("Prohibited asset count");
			NextColumnText(Inline.Span(_prohibitedAssetCount));

			ImGui.EndTable();
		}
	}

	private static unsafe void RenderTocEntriesTable()
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

				_displayedTocEntries = sorting switch
				{
					0 => sortAscending ? _displayedTocEntries.OrderBy(c => c.Name.ToLower()).ToList() : _displayedTocEntries.OrderByDescending(c => c.Name.ToLower()).ToList(),
					1 => sortAscending ? _displayedTocEntries.OrderBy(c => c.AssetType).ToList() : _displayedTocEntries.OrderByDescending(c => c.AssetType).ToList(),
					2 => sortAscending ? _displayedTocEntries.OrderBy(c => AssetContainer.IsProhibited(c.AssetType, c.Name)).ToList() : _displayedTocEntries.OrderByDescending(c => AssetContainer.IsProhibited(c.AssetType, c.Name)).ToList(),
					3 => sortAscending ? _displayedTocEntries.OrderBy(c => c.Size).ToList() : _displayedTocEntries.OrderByDescending(c => c.Size).ToList(),
					_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
				};

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < _displayedTocEntries.Count; i++)
			{
				ModBinaryTocEntry tocEntry = _displayedTocEntries[i];

				ImGui.TableNextColumn();
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
