using DevilDaggersInfo.Core.Mod;

namespace DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory.Data;

internal sealed record EffectiveAsset(ModBinaryTocEntry TocEntry, string ContainingModFileName, string? OverriddenByModFileName)
{
	public string? OverriddenByModFileName { get; set; } = OverriddenByModFileName;
}
