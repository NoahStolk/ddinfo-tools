namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

internal sealed record AudioAssetPath(string AssetName, string? AbsolutePath, float? Loudness) : IAssetPath
{
	public string? AbsolutePath { get; private set; } = AbsolutePath;
	public float? Loudness { get; private set; } = Loudness;

	public void SetPath(string? path)
	{
		AbsolutePath = path;
	}

	public void SetLoudness(float? loudness)
	{
		Loudness = loudness;
	}
}
