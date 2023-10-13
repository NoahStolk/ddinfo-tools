using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModsDirectoryWindow
{
	private static readonly ModsDirectoryLogic _logic = new();

	private static bool _showEnabledMods = true;
	private static bool _showDisabledMods = true;
	private static bool _showModsWithInvalidPrefix = true;
	private static bool _showOtherFiles;
	private static bool _showErrors = true;

	private static string? _renameErrorMessage;

	public static void Initialize()
	{
		_logic.LoadModsDirectory();
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
			_logic.LoadModsDirectory();

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
					1 => "The mod type ('audio' or 'dd')",
					2 => "The chunk count is typically the amount of game assets changed by the mod",
					3 => "The mod's file size",
					_ => string.Empty,
				};
				ImGui.SetTooltip(tooltip);
			}

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				uint sorting = sortsSpecs.Specs.ColumnUserID;
				bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;

				_logic.SortModFiles(sorting, sortAscending);

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < _logic.ModFiles.Count; i++)
			{
				ModFile modFile = _logic.ModFiles[i];

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

				bool setFocus = false;
				if (ImGui.SmallButton(Inline.Span($"Rename##{i}")))
				{
					ImGui.OpenPopup(Inline.Span($"Rename##rename_mod_file_{i}"));
					_logic.InitializeRename(modFile.FileName);
					setFocus = true;
				}

				ImGui.SetNextWindowSize(new(512, 128), ImGuiCond.Appearing);
				if (ImGui.BeginPopupModal(Inline.Span($"Rename##rename_mod_file_{i}")))
				{
					if (setFocus)
						ImGui.SetKeyboardFocusHere(0);

					ImGui.InputText("##rename", ref _logic.NewFileName, 128);

					if (_renameErrorMessage != null)
						ImGui.TextColored(Color.Red, _renameErrorMessage);

					if (ImGui.Button("OK", new(128, 0)) || ImGuiUtils.IsEnterPressed())
					{
						_renameErrorMessage = _logic.RenameModFile();
						if (_renameErrorMessage == null)
							ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}

				ImGui.SameLine();
				if (ImGui.SmallButton(Inline.Span($"Delete##{i}")))
				{
					PopupManager.ShowQuestion(
						"Delete mod file?",
						$"Are you sure you want to delete the mod file '{modFile.FileName}'?",
						() => _logic.DeleteModFile(modFile.FileName),
						() => { });
				}

				ImGui.SameLine();
				ImGui.BeginDisabled(modFile.FileType is not ModFileType.EnabledMod and not ModFileType.DisabledMod);
				if (ImGui.SmallButton(Inline.Span($"Toggle##{i}")))
					_logic.ToggleModFile(modFile.FileName);
				ImGui.EndDisabled();

				ImGui.SameLine();
				ImGui.TextColored(GetColor(modFile.FileType), modFile.FileName);

				ImGui.TableNextColumn();
				if (modFile.BinaryType.HasValue)
					ImGui.Text(EnumUtils.ModBinaryTypeNames[modFile.BinaryType.Value]);

				ImGui.TableNextColumn();
				if (modFile is { ChunkCount: not null, ProhibitedChunkCount: not null })
					ColumnTextRight(Inline.Span($"{modFile.ProhibitedChunkCount.Value} / {modFile.ChunkCount.Value} prohibited"), modFile.ProhibitedChunkCount.Value > 0 ? Color.Orange : Color.Green);

				ImGui.TableNextColumn();

				ColumnTextRight(FileSizeUtils.Format(modFile.FileSize));
			}

			ImGui.EndTable();
		}

		static void ColumnTextRight(ReadOnlySpan<char> span, Vector4? color = null)
		{
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(span).X - ImGui.GetScrollX());
			if (color.HasValue)
				ImGui.TextColored(color.Value, span);
			else
				ImGui.Text(span);
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
}