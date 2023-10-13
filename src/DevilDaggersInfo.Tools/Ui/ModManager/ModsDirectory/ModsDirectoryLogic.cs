using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;

public static class ModsDirectoryLogic
{
	private static List<ModFile> _modFiles = new();
	private static string _originalFileName = string.Empty;
	public static string NewFileName = string.Empty;

	public static IReadOnlyList<ModFile> ModFiles => _modFiles;

	public static void InitializeRename(string fileName)
	{
		_originalFileName = fileName;
		NewFileName = fileName;
	}

	public static void LoadModsDirectory()
	{
		try
		{
			_modFiles.Clear();

			string[] files = Directory.GetFiles(UserSettings.ModsDirectory);
			foreach (string file in files)
			{
				_modFiles.Add(ModFile.FromPath(file));
			}

			_modFiles = _modFiles.OrderBy(m => m.FileName).ToList();
		}
		catch (Exception ex)
		{
			PopupManager.ShowError("Error loading files in the mods directory.\n\n" + ex.Message);
			Root.Log.Error(ex, "Error loading files in the mods directory.");
		}
	}

	public static void SortModFiles(uint sorting, bool sortAscending)
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
	public static string? RenameModFile()
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
				FileType = ModFile.GetFileType(NewFileName),
			});
		}

		return null;
	}

	public static void DeleteModFile(string fileName)
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

	public static void ToggleModFile(string originalFileName)
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
				FileType = ModFile.GetFileType(newFileName),
			});
		}
	}

	public static void ToggleProhibitedAssets(string fileName)
	{
		try
		{
			string path = Path.Combine(UserSettings.ModsDirectory, fileName);
			using FileStream fs = new(path, FileMode.Open);
			using BinaryReader reader = new(fs);
			ModBinaryToc modBinaryToc = ModBinaryToc.FromReader(reader);

			bool anyProhibited = modBinaryToc.Chunks.Any(c => AssetContainer.GetIsProhibited(c.AssetType, c.Name));
			ModBinaryToc toggledToc = anyProhibited ? ModBinaryToc.DisableProhibitedAssets(modBinaryToc) : ModBinaryToc.EnableAllAssets(modBinaryToc);

			fs.Seek(12, SeekOrigin.Begin); // Skip file header
			foreach (ModBinaryChunk chunk in toggledToc.Chunks)
			{
				fs.Seek(sizeof(ushort), SeekOrigin.Current); // Skip asset type
				fs.Write(Encoding.UTF8.GetBytes(chunk.Name));
				fs.Seek(sizeof(byte), SeekOrigin.Current); // Skip null terminator
				fs.Seek(sizeof(int) * 3, SeekOrigin.Current); // Skip offset, size, and unknown
			}
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, $"Error toggling prohibited assets for file '{fileName}'.");
			PopupManager.ShowError($"Error toggling prohibited assets for file '{fileName}'.\n\n" + ex.Message);
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

		ModPreviewWindow.LoadChunks();
	}
}
