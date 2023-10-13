using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public class ModsDirectoryLogic
{
	private List<ModFile> _modFiles = new();
	private string _originalFileName = string.Empty;
	public string NewFileName = string.Empty;

	public IReadOnlyList<ModFile> ModFiles => _modFiles;

	public void InitializeRename(string fileName)
	{
		_originalFileName = fileName;
		NewFileName = fileName;
	}

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

		static ModFile ReadModFile(string filePath)
		{
			string fileName = Path.GetFileName(filePath);

			long fileSize = 0;
			try
			{
				using FileStream fs = new(filePath, FileMode.Open);
				fileSize = fs.Length;
				using BinaryReader reader = new(fs);
				ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);

				int prohibitedCount = modBinaryToc.Chunks.Count(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name));
				return new(fileName, GetModFileType(fileName), modBinaryToc.Type, modBinaryToc.Chunks.Count, prohibitedCount, fileSize);
			}
			catch (InvalidModBinaryException)
			{
				return new(fileName, ModFileType.Other, null, null, null, fileSize);
			}
			catch (Exception ex)
			{
				PopupManager.ShowError($"Error loading file '{filePath}'.\n\n" + ex.Message);
				Root.Log.Error(ex, $"Error loading file '{filePath}'.");
				return new(fileName, ModFileType.Error, null, null, null, fileSize);
			}
		}
	}

	private static ModFileType GetModFileType(string fileName)
	{
		return fileName.StartsWith("audio") || fileName.StartsWith("dd") ? ModFileType.EnabledMod :
			fileName.StartsWith("_audio") || fileName.StartsWith("_dd") ? ModFileType.DisabledMod :
			ModFileType.ModWithInvalidPrefix;
	}

	public void SortModFiles(uint sorting, bool sortAscending)
	{
		_modFiles = sorting switch
		{
			0 => sortAscending ? _modFiles.OrderBy(m => m.FileName.ToLower()).ToList() : _modFiles.OrderByDescending(m => m.FileName.ToLower()).ToList(),
			1 => sortAscending ? _modFiles.OrderBy(m => m.BinaryType).ToList() : _modFiles.OrderByDescending(m => m.BinaryType).ToList(),
			2 => sortAscending ? _modFiles.OrderBy(m => m.ProhibitedChunkCount).ToList() : _modFiles.OrderByDescending(m => m.ProhibitedChunkCount).ToList(),
			3 => sortAscending ? _modFiles.OrderBy(m => m.FileSize).ToList() : _modFiles.OrderByDescending(m => m.FileSize).ToList(),
			_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
		};
	}

	/// <summary>
	/// Renames the mod file and returns an error message if the renaming failed.
	/// </summary>
	public string? RenameModFile()
	{
		string originalPath = Path.Combine(UserSettings.ModsDirectory, _originalFileName);
		string newPath = Path.Combine(UserSettings.ModsDirectory, NewFileName);
		if (originalPath == newPath)
			return null;

		if (NewFileName.Length == 0)
			return "File name cannot be empty.";

		if (NewFileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
			return $"File '{NewFileName}' contains invalid characters.";

		if (File.Exists(newPath))
			return $"File '{NewFileName}' already exists in the mods directory.";

		try
		{
			File.Move(originalPath, newPath);
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error renaming file '{_originalFileName}' to '{NewFileName}'.");
			return $"Error renaming file '{_originalFileName}' to '{NewFileName}'.\n\n" + ex.Message;
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == _originalFileName);
		if (originalModFile == null)
		{
			Root.Log.Warning("Renamed file does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, originalModFile with
			{
				FileName = NewFileName,
				FileType = GetModFileType(NewFileName),
			});
		}

		return null;
	}

	public void DeleteModFile(string fileName)
	{
		string path = Path.Combine(UserSettings.ModsDirectory, fileName);

		try
		{
			File.Delete(path);
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error deleting file '{fileName}'.");
			PopupManager.ShowError($"Error deleting file '{fileName}'.\n\n" + ex.Message);
		}

		ModFile? modFile = _modFiles.Find(m => m.FileName == fileName);
		if (modFile == null)
			Root.Log.Warning("Deleted file does not exist in memory.");
		else
			_modFiles.Remove(modFile);
	}

	public void ToggleModFile(string originalFileName)
	{
		if (!originalFileName.StartsWith("audio") && !originalFileName.StartsWith("dd") && !originalFileName.StartsWith("_audio") && !originalFileName.StartsWith("_dd"))
			return;

		string newFileName = originalFileName.StartsWith("audio") || originalFileName.StartsWith("dd") ? $"_{originalFileName}" : originalFileName[1..];

		string originalPath = Path.Combine(UserSettings.ModsDirectory, originalFileName);
		string newPath = Path.Combine(UserSettings.ModsDirectory, newFileName);

		try
		{
			File.Move(originalPath, newPath);
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error toggling file '{originalFileName}'.");
			PopupManager.ShowError($"Error toggling file '{originalFileName}'.\n\n" + ex.Message);
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == originalFileName);
		if (originalModFile == null)
		{
			Root.Log.Warning("Renamed file does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, originalModFile with
			{
				FileName = newFileName,
				FileType = GetModFileType(newFileName),
			});
		}
	}
}
