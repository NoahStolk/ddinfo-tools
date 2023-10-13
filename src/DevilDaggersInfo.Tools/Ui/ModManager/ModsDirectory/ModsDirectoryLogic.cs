using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public class ModsDirectoryLogic
{
	private List<ModFile> _modFiles = new();

	public IReadOnlyList<ModFile> ModFiles => _modFiles;

	public void LoadModsDirectory()
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

	public void SortModFiles(uint sorting, bool sortAscending)
	{
		_modFiles = sorting switch
		{
			0 => sortAscending ? _modFiles.OrderBy(m => m.FileName).ToList() : _modFiles.OrderByDescending(m => m.FileName).ToList(),
			1 => sortAscending ? _modFiles.OrderBy(m => m.Type).ToList() : _modFiles.OrderByDescending(m => m.Type).ToList(),
			2 => sortAscending ? _modFiles.OrderBy(m => m.ChunkCount).ToList() : _modFiles.OrderByDescending(m => m.ChunkCount).ToList(),
			3 => sortAscending ? _modFiles.OrderBy(m => m.FileSize).ToList() : _modFiles.OrderByDescending(m => m.FileSize).ToList(),
			_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
		};
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
}
