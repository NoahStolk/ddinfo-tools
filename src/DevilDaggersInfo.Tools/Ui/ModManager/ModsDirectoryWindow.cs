using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager;

public static class ModsDirectoryWindow
{
	private static List<ModFile> _modFiles = new();
	private static bool _showEnabledMods = true;
	private static bool _showDisabledMods = true;
	private static bool _showModsWithInvalidPrefix = true;
	private static bool _showOtherFiles;
	private static bool _showErrors = true;

	private enum ModFileType
	{
		EnabledMod,
		DisabledMod,
		ModWithInvalidPrefix,
		Other,
		Error,
	}

	public static void Render()
	{
		ImGui.Text(Inline.Span($"""
			These are all files in mods directory. In order to make managing mods easier, the program displays which files will be loaded by the game.

			Folders are not displayed as they are always ignored by the game.

			Note: When enabling or disabling mods, the game must be restarted for the changes to take effect.

			Current mods directory: {UserSettings.ModsDirectory}
			"""));

		if (ImGui.Button("Reload"))
			LoadModsDirectory();

		ImGui.Separator();

		CheckboxColored("Enabled mods", GetColor(ModFileType.EnabledMod), ref _showEnabledMods, "To enable a mod, it must start with 'audio' or 'dd' for the game to load it.");
		CheckboxColored("Disabled mods", GetColor(ModFileType.DisabledMod), ref _showDisabledMods, "In order to easily disable a mod, the program renames the mod file to start with an underscore, so these files not loaded by the game.");
		CheckboxColored("Mods with invalid prefix", GetColor(ModFileType.ModWithInvalidPrefix), ref _showModsWithInvalidPrefix, "These are valid mod files with an invalid file name, meaning they are not loaded by the game.");
		CheckboxColored("Other files", GetColor(ModFileType.Other), ref _showOtherFiles, "These are files that are not valid mod files. Spawnsets are not considered mods by the Mod Manager.");
		CheckboxColored("Error loading file", GetColor(ModFileType.Error), ref _showErrors, "These are files that could not be loaded by the program. This could be because the program does not have the required permissions to read the file.");

		if (ImGui.BeginChild("table_child"))
			RenderTable();

		static void CheckboxColored(ReadOnlySpan<char> label, Vector4 color, ref bool value, ReadOnlySpan<char> tooltip)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, color);
			ImGui.Checkbox(label, ref value);
			ImGui.PopStyleColor();

			ImGui.SameLine();
			ImGui.Text("(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip(tooltip);
		}
	}

	private static unsafe void RenderTable()
	{
		const int columnCount = 4;
		if (ImGui.BeginTable("mod_file_table", columnCount, ImGuiTableFlags.Resizable | ImGuiTableFlags.Sortable))
		{
			ImGui.TableSetupColumn("File name", ImGuiTableColumnFlags.DefaultSort, 256, 0);
			ImGui.TableSetupColumn("Mod type", ImGuiTableColumnFlags.None, 128, 1);
			ImGui.TableSetupColumn("Chunk count", ImGuiTableColumnFlags.None, 64, 2);
			ImGui.TableSetupColumn("File size", ImGuiTableColumnFlags.None, 64, 3);
			ImGui.TableHeadersRow();

			for (int i = 0; i < columnCount; i++)
			{
				if (!ImGui.TableSetColumnIndex(i))
					continue;

				ImGui.TableHeader(ImGui.TableGetColumnName(i));
				if (!ImGui.IsItemHovered())
					continue;

				string tooltip = i switch
				{
					0 => "The name of the file in the mods folder",
					1 => "The mod type (audio or dd, core mods are not supported by the game)",
					2 => "The chunk count is typically the amount of game assets changed by the mod",
					3 => "The file size in bytes",
					_ => string.Empty,
				};
				ImGui.SetTooltip(tooltip);
			}

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				_modFiles = sorting switch
				{
					0 => sortAscending ? _modFiles.OrderBy(m => m.FileName).ToList() : _modFiles.OrderByDescending(m => m.FileName).ToList(),
					1 => sortAscending ? _modFiles.OrderBy(m => m.Type).ToList() : _modFiles.OrderByDescending(m => m.Type).ToList(),
					2 => sortAscending ? _modFiles.OrderBy(m => m.ChunkCount).ToList() : _modFiles.OrderByDescending(m => m.ChunkCount).ToList(),
					3 => sortAscending ? _modFiles.OrderBy(m => m.FileSize).ToList() : _modFiles.OrderByDescending(m => m.FileSize).ToList(),
					_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
				};

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < _modFiles.Count; i++)
			{
				ModFile modFile = _modFiles[i];

				if (!_showEnabledMods && modFile.FileType == ModFileType.EnabledMod)
					continue;

				if (!_showDisabledMods && modFile.FileType == ModFileType.DisabledMod)
					continue;

				if (!_showModsWithInvalidPrefix && modFile.FileType == ModFileType.ModWithInvalidPrefix)
					continue;

				if (!_showOtherFiles && modFile.FileType == ModFileType.Other)
					continue;

				if (!_showErrors && modFile.FileType == ModFileType.Error)
					continue;

				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				ImGui.TextColored(GetColor(modFile.FileType), modFile.FileName);

				ImGui.TableNextColumn();
				if (modFile.Type.HasValue)
					ImGui.Text(EnumUtils.ModBinaryTypeNames[modFile.Type.Value]);

				ImGui.TableNextColumn();
				if (modFile.ChunkCount.HasValue)
					ColumnTextRight(Inline.Span(modFile.ChunkCount.Value));

				ImGui.TableNextColumn();

				ColumnTextRight(FileSizeUtils.Format(modFile.FileSize));
			}

			ImGui.EndTable();
		}
	}

	private static void ColumnTextRight(ReadOnlySpan<char> span)
	{
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(span).X - ImGui.GetScrollX());
		ImGui.Text(span);
	}

	public static void LoadModsDirectory()
	{
		try
		{
			_modFiles.Clear();

			string[] files = Directory.GetFiles(UserSettings.ModsDirectory);
			foreach (string file in files)
			{
				_modFiles.Add(ReadModFile(file));
			}

			_modFiles = _modFiles.OrderBy(m => m.FileName).ToList();
		}
		catch (Exception ex)
		{
			PopupManager.ShowError("Error loading files in the mods directory.\n\n" + ex.Message);
			Root.Log.Error(ex, "Error loading files in the mods directory.");
		}
	}

	private static ModFile ReadModFile(string filePath)
	{
		string fileName = Path.GetFileName(filePath);

		long fileSize = 0;
		try
		{
			using FileStream fs = new(filePath, FileMode.Open);
			fileSize = fs.Length;
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);

			ModFileType fileType =
				fileName.StartsWith("audio") || fileName.StartsWith("dd") ? ModFileType.EnabledMod :
				fileName.StartsWith("_audio") || fileName.StartsWith("_dd") ? ModFileType.DisabledMod :
				ModFileType.ModWithInvalidPrefix;

			return new(fileName, fileType, modBinaryToc.Type, modBinaryToc.Chunks.Count, fileSize);
		}
		catch (InvalidModBinaryException)
		{
			return new(fileName, ModFileType.Other, null, null, fileSize);
		}
		catch (Exception ex)
		{
			PopupManager.ShowError($"Error loading file '{filePath}'.\n\n" + ex.Message);
			Root.Log.Error(ex, $"Error loading file '{filePath}'.");
			return new(fileName, ModFileType.Error, null, null, fileSize);
		}
	}

	private static Vector4 GetColor(ModFileType modFileType)
	{
		return modFileType switch
		{
			ModFileType.Error => new(0.8f, 0.2f, 0.2f, 1),
			ModFileType.Other => new(0.4f, 0.4f, 0.4f, 1),
			ModFileType.ModWithInvalidPrefix => new(0.8f, 0.5f, 0.2f, 1),
			ModFileType.DisabledMod => new(0.8f, 0.8f, 0.8f, 1),
			ModFileType.EnabledMod => new(0.2f, 0.8f, 0.2f, 1),
			_ => new(1),
		};
	}

	private sealed record ModFile(string FileName, ModFileType FileType, ModBinaryType? Type, int? ChunkCount, long FileSize);
}
