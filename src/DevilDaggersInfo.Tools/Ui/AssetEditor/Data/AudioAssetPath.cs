using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AudioAssetPath(AssetType AssetType, string AssetName, string? AbsolutePath, float? Loudness) : IAssetPath
{
	public string? AbsolutePath { get; private set; } = AbsolutePath;

	public float? Loudness { get; private set; } = Loudness;

	public void SetPath(string? path)
	{
		if (path != null)
			AbsolutePath = path;
	}

	public void SetLoudness(float? loudness)
	{
		if (loudness != null)
			Loudness = loudness;
	}
}
