using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AssetPath(AssetType AssetType, string AssetName, string AbsolutePath)
{
	public string AbsolutePath { get; private set; } = AbsolutePath;

	public void SetPath(string? path)
	{
		if (path != null)
			AbsolutePath = path;
	}
}
