using DevilDaggersInfo.Core.Asset;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AudioAssetPath(string AssetName, string? AbsolutePath, float? Loudness) : IAssetPath
{
	[JsonIgnore]
	public AssetType AssetType => AssetType.Audio;

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
