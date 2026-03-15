using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;

internal sealed record ModFile(string FileName, ModFileType FileType, ModBinaryType? BinaryType, int? AssetCount, int? ProhibitedAssetCount, long FileSize)
{
	public static ModFileType GetFileType(string fileName)
	{
		return fileName.StartsWith("audio") || fileName.StartsWith("dd") ? ModFileType.EnabledMod :
			fileName.StartsWith("_audio") || fileName.StartsWith("_dd") ? ModFileType.DisabledMod :
			ModFileType.ModWithInvalidPrefix;
	}

	// TODO: Move to instance class (factory) so we can use DI and not depend on static methods.
	public static ModFile FromPath(PopupManager popupManager, string filePath)
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
			return new ModFile(fileName, GetFileType(fileName), modBinaryToc.Type, modBinaryToc.Entries.Count, prohibitedCount, fileSize);
		}
		catch (InvalidModBinaryException)
		{
			return new ModFile(fileName, ModFileType.Other, null, null, null, fileSize);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			popupManager.ShowError($"Error loading file '{filePath}'.", ex);
			Root.Log.Error(ex, $"Error loading file '{filePath}'.");
			return new ModFile(fileName, ModFileType.Error, null, null, null, fileSize);
		}
	}
}
