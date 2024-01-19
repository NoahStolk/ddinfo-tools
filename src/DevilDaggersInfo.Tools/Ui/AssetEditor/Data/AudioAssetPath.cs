using DevilDaggersInfo.Core.Asset;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record AudioAssetPath : IAssetPath
{
	public AudioAssetPath(string assetName, string? absolutePath, float? loudness)
	{
		AssetName = assetName;
		AbsolutePath = absolutePath;
		Loudness = loudness;

		DefaultLoudness = GetAudioAssetInfo(assetName)?.DefaultLoudness ?? 1;
	}

	[JsonIgnore]
	public AssetType AssetType => AssetType.Audio;
	public string AssetName { get; }
	public string? AbsolutePath { get; private set; }
	public float? Loudness { get; private set; }

	[JsonIgnore]
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
