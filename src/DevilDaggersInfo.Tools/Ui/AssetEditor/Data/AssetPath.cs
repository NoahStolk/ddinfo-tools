using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AssetPath
{
	public required string AssetName { get; init; }

	public required AssetType AssetType { get; init; }

	public required string AbsolutePath { get; init; }
}
