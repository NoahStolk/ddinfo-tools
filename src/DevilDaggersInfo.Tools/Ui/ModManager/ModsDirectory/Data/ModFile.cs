using DevilDaggersInfo.Core.Mod;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;

public sealed record ModFile(string FileName, ModFileType FileType, ModBinaryType? BinaryType, int? ChunkCount, int? ProhibitedChunkCount, long FileSize);
