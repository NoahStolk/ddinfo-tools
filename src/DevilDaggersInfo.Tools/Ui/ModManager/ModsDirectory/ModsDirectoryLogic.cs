using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using Serilog.Core;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

internal sealed class ModsDirectoryLogic(PopupManager popupManager, ModManagerState modManagerState, UserSettings userSettings, Logger logger)
{
	private List<ModFile> _modFiles = [];
	private string _originalFileName = string.Empty;
	public string NewFileName = string.Empty;

	public IReadOnlyList<ModFile> ModFiles => _modFiles;
	public bool IsLoading { get; private set; }

	public void InitializeRename(string fileName)
	{
		_originalFileName = fileName;
		NewFileName = fileName;
	}

	public void LoadModsDirectory()
	{
		Task.Run(async () =>
		{
			if (IsLoading)
				return;

			IsLoading = true;

			await Task.Yield();

			try
			{
				_modFiles.Clear();

				string[] files = Directory.GetFiles(userSettings.ModsDirectory);
				foreach (string file in files)
				{
					_modFiles.Add(CreateModFile(file));
				}
			}
			catch (Exception ex) when (ex.IsFileIoException())
			{
				popupManager.ShowError("Error loading files in the mods directory.", ex);
				logger.Error(ex, "Error loading files in the mods directory.");
			}

			ModInstallationWindow.LoadEffectiveAssets();

			IsLoading = false;

			// TODO: Sort by current sorting.
			SortModFiles(0, true);
		});
	}

	public void SortModFiles(uint sorting, bool sortAscending)
	{
		if (IsLoading)
			return; // Cannot sort while loading because List<T> is not thread-safe.

		_modFiles = sorting switch
		{
			0 => sortAscending ? _modFiles.OrderBy(m => m.FileName.ToLower()).ToList() : _modFiles.OrderByDescending(m => m.FileName.ToLower()).ToList(),
			1 => sortAscending ? _modFiles.OrderBy(m => m.BinaryType).ToList() : _modFiles.OrderByDescending(m => m.BinaryType).ToList(),
			2 => sortAscending ? _modFiles.OrderBy(m => m.ProhibitedAssetCount).ToList() : _modFiles.OrderByDescending(m => m.ProhibitedAssetCount).ToList(),
			3 => sortAscending ? _modFiles.OrderBy(m => m.FileSize).ToList() : _modFiles.OrderByDescending(m => m.FileSize).ToList(),
			_ => throw new InvalidOperationException($"Invalid sorting column '{sorting}'."),
		};
	}

	/// <summary>
	/// Renames the mod file and returns an error message if the renaming failed.
	/// </summary>
	public string? RenameModFile()
	{
		if (IsLoading)
			return null;

		string originalPath = Path.Combine(userSettings.ModsDirectory, _originalFileName);
		string newPath = Path.Combine(userSettings.ModsDirectory, NewFileName);
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
		catch (Exception ex) when (ex.IsFileIoException())
		{
			logger.Error(ex, $"Error renaming file '{_originalFileName}' to '{NewFileName}'.");
			return $"Error renaming file '{_originalFileName}' to '{NewFileName}'.\n\n" + ex.Message;
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == _originalFileName);
		if (originalModFile == null)
		{
			logger.Warning("Renamed file does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, originalModFile with
			{
				FileName = NewFileName,
				FileType = ModFile.GetFileType(NewFileName),
			});
		}

		ModInstallationWindow.LoadEffectiveAssets();
		modManagerState.UpdateIfSelected(_originalFileName, NewFileName);

		return null;
	}

	public void DeleteModFile(string fileName)
	{
		if (IsLoading)
			return;

		string path = Path.Combine(userSettings.ModsDirectory, fileName);

		try
		{
			File.Delete(path);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			logger.Error(ex, $"Error deleting file '{fileName}'.");
			popupManager.ShowError($"Error deleting file '{fileName}'.", ex);
		}

		ModFile? modFile = _modFiles.Find(m => m.FileName == fileName);
		if (modFile == null)
			logger.Warning("Deleted file does not exist in memory.");
		else
			_modFiles.Remove(modFile);

		ModInstallationWindow.LoadEffectiveAssets();
		modManagerState.DeleteIfSelected(fileName);
	}

	public void ToggleModFile(string originalFileName)
	{
		if (IsLoading)
			return;

		if (!originalFileName.StartsWith("audio") && !originalFileName.StartsWith("dd") && !originalFileName.StartsWith("_audio") && !originalFileName.StartsWith("_dd"))
			return;

		string newFileName = originalFileName.StartsWith("audio") || originalFileName.StartsWith("dd") ? $"_{originalFileName}" : originalFileName[1..];

		string originalPath = Path.Combine(userSettings.ModsDirectory, originalFileName);
		string newPath = Path.Combine(userSettings.ModsDirectory, newFileName);

		try
		{
			File.Move(originalPath, newPath);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			logger.Error(ex, $"Error toggling file '{originalFileName}'.");
			popupManager.ShowError($"Error toggling file '{originalFileName}'.", ex);
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == originalFileName);
		if (originalModFile == null)
		{
			logger.Warning("Renamed file does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, originalModFile with
			{
				FileName = newFileName,
				FileType = ModFile.GetFileType(newFileName),
			});
		}

		ModInstallationWindow.LoadEffectiveAssets();
		modManagerState.UpdateIfSelected(originalFileName, newFileName);
	}

	public void ToggleAssets(string fileName, Func<ModBinaryToc, ModBinaryToc> toggleFunction)
	{
		if (IsLoading)
			return;

		try
		{
			string path = Path.Combine(userSettings.ModsDirectory, fileName);
			using FileStream fs = new(path, FileMode.Open, FileAccess.ReadWrite);
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);
			ModBinaryToc toggledToc = toggleFunction(modBinaryToc);

			OverwriteToc(fs, toggledToc);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			logger.Error(ex, $"Error toggling prohibited assets for file '{fileName}'.");
			popupManager.ShowError($"Error toggling prohibited assets for file '{fileName}'.", ex);
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == fileName);
		if (originalModFile == null)
		{
			logger.Warning("File with toggled assets does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, CreateModFile(Path.Combine(userSettings.ModsDirectory, fileName)));
		}

		modManagerState.LoadTocEntries();
		ModInstallationWindow.LoadEffectiveAssets();
	}

	// TODO: This should be added to DevilDaggersInfo.Core.Mod.
	private static void OverwriteToc(FileStream fs, ModBinaryToc toggledToc)
	{
		fs.Seek(12, SeekOrigin.Begin); // Skip file header
		foreach (ModBinaryTocEntry tocEntry in toggledToc.Entries)
		{
			fs.Seek(sizeof(ushort), SeekOrigin.Current); // Skip asset type
			fs.Write(Encoding.UTF8.GetBytes(tocEntry.Name));
			fs.Seek(sizeof(byte), SeekOrigin.Current); // Skip null terminator
			fs.Seek(sizeof(int) * 3, SeekOrigin.Current); // Skip offset, size, and unknown
		}
	}

	private ModFile CreateModFile(string filePath)
	{
		string fileName = Path.GetFileName(filePath);

		long fileSize = 0;
		try
		{
			using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
			fileSize = fs.Length;
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);

			int prohibitedCount = modBinaryToc.Entries.Count(c => AssetContainer.IsProhibited(c.AssetType, c.Name));
			return new ModFile(fileName, ModFile.GetFileType(fileName), modBinaryToc.Type, modBinaryToc.Entries.Count, prohibitedCount, fileSize);
		}
		catch (InvalidModBinaryException)
		{
			return new ModFile(fileName, ModFileType.Other, null, null, null, fileSize);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			popupManager.ShowError($"Error loading file '{filePath}'.", ex);
			logger.Error(ex, $"Error loading file '{filePath}'.");
			return new ModFile(fileName, ModFileType.Error, null, null, null, fileSize);
		}
	}
}
