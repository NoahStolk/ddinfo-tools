using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AudioAssetPath : IAssetPath
{
	public AudioAssetPath(AssetType assetType, string assetName, string? absolutePath, float? loudness)
	{
		AssetType = assetType;
		AssetName = assetName;
		AbsolutePath = absolutePath;
		Loudness = loudness;

		DefaultLoudness = GetAudioAssetInfo(assetName)?.DefaultLoudness ?? 1;
	}

	public AssetType AssetType { get; init; }
	public string AssetName { get; init; }
	public string? AbsolutePath { get; private set; }
	public float? Loudness { get; private set; }

	public float DefaultLoudness { get; }

	private static AudioAssetInfo? GetAudioAssetInfo(string assetName)
	{
		for (int i = 0; i < AudioAudio.All.Count; i++)
		{
			AudioAssetInfo audio = AudioAudio.All[i];
			if (assetName == audio.AssetName)
				return audio;
		}

		return null;
	}

	public void SetPath(string? path)
	{
		AbsolutePath = path;
	}

	public void SetLoudness(float? loudness)
	{
		Loudness = loudness;
	}
}
