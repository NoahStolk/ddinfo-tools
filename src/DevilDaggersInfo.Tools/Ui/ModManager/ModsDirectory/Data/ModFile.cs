using DevilDaggersInfo.Core.Mod;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;

internal sealed record ModFile(string FileName, ModFileType FileType, ModBinaryType? BinaryType, int? AssetCount, int? ProhibitedAssetCount, long FileSize)
{
	public static ModFileType GetFileType(string fileName)
	{
		return fileName.StartsWith("audio") || fileName.StartsWith("dd") ? ModFileType.EnabledMod :
			fileName.StartsWith("_audio") || fileName.StartsWith("_dd") ? ModFileType.DisabledMod :
			ModFileType.ModWithInvalidPrefix;
	}
}
