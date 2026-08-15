using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using Serilog;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

internal sealed class ModsDirectoryLogic(
	PopupManager popupManager,
	ModManagerState modManagerState,
	ILogger logger,
	UserSettings userSettings)
{
	private List<ModFile> _modFiles = [];
	private string _originalFileName = string.Empty;
	public string NewFileName = string.Empty;

	public Dictionary<string, List<EffectiveAsset>> EffectiveAssets { get; private set; } = new();
	public int ActiveAssets { get; private set; }
	public int ActiveProhibitedAssets { get; private set; }

	public List<string> Errors { get; } = [];

	public IReadOnlyList<ModFile> ModFiles => _modFiles;
	public bool IsLoading { get; private set; }

	private void LoadEffectiveAssets()
	{
		EffectiveAssets.Clear();
		Errors.Clear();

		string[] filePaths = Directory.GetFiles(userSettings.ModsDirectory);
		List<Mod> mods = [];
		foreach (string filePath in filePaths)
		{
			string fileName = Path.GetFileName(filePath);
			if (!fileName.StartsWith("audio") && !fileName.StartsWith("dd"))
				continue;

			try
			{
				using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
				using BinaryReader reader = new(fs);
				mods.Add(new Mod(ModBinaryToc.FromReader(reader), fileName));
			}
			catch (InvalidModBinaryException)
			{
				// Ignore.
			}
			catch (Exception ex)
			{
				Errors.Add($"Error loading file '{filePath}'.");
				logger.Error(ex, "Error loading file '{FilePath}'.", filePath);
			}
		}

		List<EffectiveAsset> effectiveAssets = [];
		foreach (Mod mod in mods.OrderBy(t => t.FileName))
		{
			foreach (ModBinaryTocEntry tocEntry in mod.Toc.Entries)
			{
				List<EffectiveAsset> existingAssets = effectiveAssets.Where(c => c.TocEntry.AssetType == tocEntry.AssetType && c.TocEntry.Name == tocEntry.Name).ToList();
				foreach (EffectiveAsset existingAsset in existingAssets)
					existingAsset.OverriddenByModFileName = mod.FileName;

				effectiveAssets.Add(new EffectiveAsset(tocEntry, mod.FileName, null));
			}
		}

		foreach (EffectiveAsset effectiveAsset in effectiveAssets.OrderBy(c => c.TocEntry.AssetType).ThenBy(c => c.TocEntry.Name))
		{
			if (!EffectiveAssets.ContainsKey(effectiveAsset.ContainingModFileName))
				EffectiveAssets.Add(effectiveAsset.ContainingModFileName, []);

			EffectiveAssets[effectiveAsset.ContainingModFileName].Add(effectiveAsset);
		}

		EffectiveAssets = EffectiveAssets.OrderByDescending(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		ActiveAssets = EffectiveAssets.Sum(kvp => kvp.Value.Count(c => c.OverriddenByModFileName == null && c.TocEntry.IsEnabled));
		ActiveProhibitedAssets = EffectiveAssets.Sum(kvp => kvp.Value.Count(c => c.OverriddenByModFileName == null && c.TocEntry.IsEnabled && AssetContainer.IsProhibited(c.TocEntry.AssetType, c.TocEntry.Name)));
	}

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
					_modFiles.Add(CreateModFileFromPath(file));
				}
			}
			catch (Exception ex) when (ex.IsFileIoException())
			{
				popupManager.ShowError("Error loading files in the mods directory.", ex);
				logger.Error(ex, "Error loading files in the mods directory.");
			}

			LoadEffectiveAssets();

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
			logger.Error(ex, "Error renaming file '{OriginalFileName}' to '{NewFileName}'.", _originalFileName, NewFileName);
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

		LoadEffectiveAssets();
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
			logger.Error(ex, "Error deleting file '{FileName}'.", fileName);
			popupManager.ShowError($"Error deleting file '{fileName}'.", ex);
		}

		ModFile? modFile = _modFiles.Find(m => m.FileName == fileName);
		if (modFile == null)
			logger.Warning("Deleted file does not exist in memory.");
		else
			_modFiles.Remove(modFile);

		LoadEffectiveAssets();
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
			logger.Error(ex, "Error toggling file '{FileName}'.", originalFileName);
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

		LoadEffectiveAssets();
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
			logger.Error(ex, "Error toggling prohibited assets for file '{FileName}'.", fileName);
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
			_modFiles.Insert(originalIndex, CreateModFileFromPath(Path.Combine(userSettings.ModsDirectory, fileName)));
		}

		modManagerState.LoadTocEntries();
		LoadEffectiveAssets();
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

	private ModFile CreateModFileFromPath(string filePath)
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
			logger.Error(ex, "Error loading file '{FilePath}'.", filePath);
			return new ModFile(fileName, ModFileType.Error, null, null, null, fileSize);
		}
	}

	private sealed record Mod(ModBinaryToc Toc, string FileName);
}
