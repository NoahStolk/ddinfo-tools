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
	private static List<ModBinaryChunk> _displayedChunks = new();
	private static ModBinaryType? _binaryType;
	private static int _chunkCount;
	private static int _prohibitedChunkCount;
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
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);
			_binaryType = modBinaryToc.Type;
			_chunkCount = modBinaryToc.Chunks.Count;
			_prohibitedChunkCount = modBinaryToc.Chunks.Count(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name));
			_displayedChunks.Clear();
			_displayedChunks.AddRange(modBinaryToc.Chunks);
		}
		catch (InvalidModBinaryException)
		{
			ClearState();
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error loading mod binary '{_selectedFileName}'.");
			PopupManager.ShowError($"Error loading mod binary '{_selectedFileName}'.\n\n" + ex.Message);
			ClearState();
		}
	}

	private static void ClearState()
	{
		_displayedChunks.Clear();
		_binaryType = null;
		_chunkCount = 0;
		_prohibitedChunkCount = 0;
		_selectedFileName = null;
		_modFileSize = null;
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(640, 360));
		if (ImGui.Begin("Mod Preview", ImGuiWindowFlags.NoCollapse))
		{
			ImGui.PopStyleVar();

			if (_selectedFileName == null || _binaryType == null)
			{
				ImGui.Text("Select a valid mod from the Mod Manager window to preview its contents.");
			}
			else
			{
				RenderFileInfoTable(_binaryType.Value);

				if (ImGui.Button("Toggle prohibited"))
					ModsDirectoryLogic.ToggleProhibitedAssets(_selectedFileName);

				RenderChunksTable();
			}
		}
		else
		{
			ImGui.PopStyleVar();
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
			NextColumnText(Inline.Span(_chunkCount));

			NextColumnText("Prohibited asset count");
			NextColumnText(Inline.Span(_prohibitedChunkCount));

			ImGui.EndTable();
		}
	}

	private static unsafe void RenderChunksTable()
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

				_displayedChunks = sorting switch
				{
					0 => sortAscending ? _displayedChunks.OrderBy(c => c.Name.ToLower()).ToList() : _displayedChunks.OrderByDescending(c => c.Name.ToLower()).ToList(),
					1 => sortAscending ? _displayedChunks.OrderBy(c => c.AssetType).ToList() : _displayedChunks.OrderByDescending(c => c.AssetType).ToList(),
					2 => sortAscending ? _displayedChunks.OrderBy(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name)).ToList() : _displayedChunks.OrderByDescending(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name)).ToList(),
					3 => sortAscending ? _displayedChunks.OrderBy(c => c.Size).ToList() : _displayedChunks.OrderByDescending(c => c.Size).ToList(),
					_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
				};

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < _displayedChunks.Count; i++)
			{
				ModBinaryChunk chunk = _displayedChunks[i];

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
