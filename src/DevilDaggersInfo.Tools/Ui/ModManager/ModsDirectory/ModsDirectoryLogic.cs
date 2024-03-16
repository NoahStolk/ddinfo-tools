using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModsDirectoryLogic
{
	private static List<ModFile> _modFiles = [];
	private static string _originalFileName = string.Empty;
	public static string NewFileName = string.Empty;

	public static IReadOnlyList<ModFile> ModFiles => _modFiles;
	public static bool IsLoading { get; private set; }

	public static void InitializeRename(string fileName)
	{
		_originalFileName = fileName;
		NewFileName = fileName;
	}

	public static void LoadModsDirectory()
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

				string[] files = Directory.GetFiles(UserSettings.ModsDirectory);
				foreach (string file in files)
				{
					_modFiles.Add(ModFile.FromPath(file));
				}
			}
			catch (Exception ex) when (ex.IsFileIoException())
			{
				PopupManager.ShowError("Error loading files in the mods directory.", ex);
				Root.Log.Error(ex, "Error loading files in the mods directory.");
			}

			ModInstallationWindow.LoadEffectiveAssets();

			IsLoading = false;

			// TODO: Sort by current sorting.
			SortModFiles(0, true);
		});
	}

	public static void SortModFiles(uint sorting, bool sortAscending)
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
	public static string? RenameModFile()
	{
		if (IsLoading)
			return null;

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
		catch (Exception ex) when (ex.IsFileIoException())
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
				FileType = ModFile.GetFileType(NewFileName),
			});
		}

		ModInstallationWindow.LoadEffectiveAssets();
		ModPreviewWindow.UpdateIfSelected(_originalFileName, NewFileName);

		return null;
	}

	public static void DeleteModFile(string fileName)
	{
		if (IsLoading)
			return;

		string path = Path.Combine(UserSettings.ModsDirectory, fileName);

		try
		{
			File.Delete(path);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			Root.Log.Error(ex, $"Error deleting file '{fileName}'.");
			PopupManager.ShowError($"Error deleting file '{fileName}'.", ex);
		}

		ModFile? modFile = _modFiles.Find(m => m.FileName == fileName);
		if (modFile == null)
			Root.Log.Warning("Deleted file does not exist in memory.");
		else
			_modFiles.Remove(modFile);

		ModInstallationWindow.LoadEffectiveAssets();
		ModPreviewWindow.DeleteIfSelected(fileName);
	}

	public static void ToggleModFile(string originalFileName)
	{
		if (IsLoading)
			return;

		if (!originalFileName.StartsWith("audio") && !originalFileName.StartsWith("dd") && !originalFileName.StartsWith("_audio") && !originalFileName.StartsWith("_dd"))
			return;

		string newFileName = originalFileName.StartsWith("audio") || originalFileName.StartsWith("dd") ? $"_{originalFileName}" : originalFileName[1..];

		string originalPath = Path.Combine(UserSettings.ModsDirectory, originalFileName);
		string newPath = Path.Combine(UserSettings.ModsDirectory, newFileName);

		try
		{
			File.Move(originalPath, newPath);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			Root.Log.Error(ex, $"Error toggling file '{originalFileName}'.");
			PopupManager.ShowError($"Error toggling file '{originalFileName}'.", ex);
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
				FileType = ModFile.GetFileType(newFileName),
			});
		}

		ModInstallationWindow.LoadEffectiveAssets();
		ModPreviewWindow.UpdateIfSelected(originalFileName, newFileName);
	}

	public static void ToggleAssets(string fileName, Func<ModBinaryToc, ModBinaryToc> toggleFunction)
	{
		if (IsLoading)
			return;

		try
		{
			string path = Path.Combine(UserSettings.ModsDirectory, fileName);
			using FileStream fs = new(path, FileMode.Open, FileAccess.ReadWrite);
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);
			ModBinaryToc toggledToc = toggleFunction(modBinaryToc);

			OverwriteToc(fs, toggledToc);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			Root.Log.Error(ex, $"Error toggling prohibited assets for file '{fileName}'.");
			PopupManager.ShowError($"Error toggling prohibited assets for file '{fileName}'.", ex);
		}

		ModFile? originalModFile = _modFiles.Find(m => m.FileName == fileName);
		if (originalModFile == null)
		{
			Root.Log.Warning("File with toggled assets does not exist in memory.");
		}
		else
		{
			int originalIndex = _modFiles.IndexOf(originalModFile);
			_modFiles.Remove(originalModFile);
			_modFiles.Insert(originalIndex, ModFile.FromPath(Path.Combine(UserSettings.ModsDirectory, fileName)));
		}

		ModPreviewWindow.LoadTocEntries();
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
}
