using DevilDaggersInfo.Core.Mod;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;

public sealed record ModFile(string FileName, ModFileType FileType, ModBinaryType? Type, int? ChunkCount, long FileSize);
