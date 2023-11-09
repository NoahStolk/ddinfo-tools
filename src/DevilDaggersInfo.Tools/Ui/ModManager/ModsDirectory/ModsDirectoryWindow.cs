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
	private static bool _showEnabledMods = true;
	private static bool _showDisabledMods = true;
	private static bool _showModsWithInvalidPrefix = true;
	private static bool _showOtherFiles;
	private static bool _showErrors = true;

	private static string? _renameErrorMessage;

	public static void Render()
	{
		ImGui.Text(Inline.Span($"""
			These are all files in mods directory. In order to make managing mods easier, the program displays which files will be loaded by the game.

			Folders are not displayed as they are always ignored by the game.

			Note: When enabling or disabling mods, the game must be restarted for the changes to take effect.

			Current mods directory: {UserSettings.ModsDirectory}
			"""));

		if (ImGui.Button("Reload"))
			ModsDirectoryLogic.LoadModsDirectory();

		ImGui.Separator();

		CheckboxColored("Enabled mods", GetColor(ModFileType.EnabledMod), ref _showEnabledMods, "To enable a mod, it must start with 'audio' or 'dd' for the game to load it.");
		CheckboxColored("Disabled mods", GetColor(ModFileType.DisabledMod), ref _showDisabledMods, "In order to easily disable a mod, the program renames the mod file to start with an underscore, so these files not loaded by the game.");
		CheckboxColored("Mods with invalid prefix", GetColor(ModFileType.ModWithInvalidPrefix), ref _showModsWithInvalidPrefix, "These are valid mod files with an invalid file name, meaning they are not loaded by the game.");
		CheckboxColored("Other files", GetColor(ModFileType.Other), ref _showOtherFiles, "These are files that are not valid mod files. Spawnsets are not considered mods by the Mod Manager.");
		CheckboxColored("Error loading file", GetColor(ModFileType.Error), ref _showErrors, "These are files that could not be loaded by the program. This could be because the program does not have the required permissions to read the file.");

		if (ImGui.BeginChild("table_child"))
		{
			if (ModsDirectoryLogic.IsLoading)
				ImGui.Text("Loading...");
			else
				RenderTable();
		}

		ImGui.EndChild(); // End table_child

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
			ImGui.TableSetupColumn("Prohibited", ImGuiTableColumnFlags.None, 64, 2);
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
					2 => """
						Whether prohibited assets are included in the mod

						Prohibited mods prevent scores of 1000 seconds or higher from being submitted to the official leaderboards.

						It is completely safe to use these mods if you're not going for a 1000+ score.
						""",
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

				ModsDirectoryLogic.SortModFiles(sorting, sortAscending);

				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < ModsDirectoryLogic.ModFiles.Count; i++)
			{
				ModFile modFile = ModsDirectoryLogic.ModFiles[i];
				if (!_showEnabledMods && modFile.FileType == ModFileType.EnabledMod ||
				    !_showDisabledMods && modFile.FileType == ModFileType.DisabledMod ||
				    !_showModsWithInvalidPrefix && modFile.FileType == ModFileType.ModWithInvalidPrefix ||
				    !_showOtherFiles && modFile.FileType == ModFileType.Other ||
				    !_showErrors && modFile.FileType == ModFileType.Error)
				{
					continue;
				}

				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, modFile.FileName == ModPreviewWindow.SelectedFileName ? 0x4400ffffU : modFile.FileType == ModFileType.EnabledMod ? 0x4400ff00U : 0x00000000U);

				bool setFocus = false;
				if (ImGui.SmallButton(Inline.Span($"Rename##{i}")))
				{
					ImGui.OpenPopup(Inline.Span($"Rename##rename_mod_file_{i}"));
					ModsDirectoryLogic.InitializeRename(modFile.FileName);
					setFocus = true;
				}

				ImGui.SetNextWindowSize(new(512, 128), ImGuiCond.Appearing);
				if (ImGui.BeginPopupModal(Inline.Span($"Rename##rename_mod_file_{i}")))
				{
					if (setFocus)
						ImGui.SetKeyboardFocusHere(0);

					ImGui.InputText("##rename", ref ModsDirectoryLogic.NewFileName, 128);

					if (_renameErrorMessage != null)
						ImGui.TextColored(Color.Red, _renameErrorMessage);

					if (ImGui.Button("OK", new(128, 0)) || ImGuiUtils.IsEnterPressed())
					{
						_renameErrorMessage = ModsDirectoryLogic.RenameModFile();
						if (_renameErrorMessage == null)
							ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}

				ImGui.SameLine();
				if (ImGui.SmallButton(Inline.Span($"Delete##{i}")))
				{
					// Only capture the fileName variable when needed.
					// If we use modFile.FileName directly in the onConfirm lambda, it will capture the variable every frame, which allocates a couple kilobytes of memory per frame.
					string fileName = modFile.FileName;
					PopupManager.ShowQuestion(
						"Delete mod file?",
						$"Are you sure you want to delete the mod file '{modFile.FileName}'?",
						() => ModsDirectoryLogic.DeleteModFile(fileName),
						static () => { });
				}

				ImGui.SameLine();
				ImGui.BeginDisabled(modFile.FileType is not ModFileType.EnabledMod and not ModFileType.DisabledMod);
				if (ImGui.SmallButton(Inline.Span($"Toggle##{i}")))
					ModsDirectoryLogic.ToggleModFile(modFile.FileName);
				ImGui.EndDisabled();

				ImGui.SameLine();
				ImGui.PushStyleColor(ImGuiCol.Text, GetColor(modFile.FileType));
				bool temp = false;
				if (ImGui.Selectable(modFile.FileName, ref temp, ImGuiSelectableFlags.SpanAllColumns))
					ModPreviewWindow.SelectedFileName = ModPreviewWindow.SelectedFileName == modFile.FileName ? null : modFile.FileName;
				ImGui.PopStyleColor();

				ImGui.TableNextColumn();
				if (modFile.BinaryType.HasValue)
					ImGui.Text(EnumUtils.ModBinaryTypeNames[modFile.BinaryType.Value]);

				ImGui.TableNextColumn();
				if (modFile is { ChunkCount: not null, ProhibitedChunkCount: not null })
				{
					if (modFile.ProhibitedChunkCount.Value > 0)
						ImGui.TextColored(Color.Orange, Inline.Span($"Prohibited ({modFile.ProhibitedChunkCount.Value} of {modFile.ChunkCount.Value})"));
					else
						ImGui.TextColored(Color.Green, "OK");

					if (ImGui.IsItemHovered())
						ImGui.SetTooltip(Inline.Span($"{modFile.ProhibitedChunkCount.Value} out of {modFile.ChunkCount.Value} assets prohibited"));
				}

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
