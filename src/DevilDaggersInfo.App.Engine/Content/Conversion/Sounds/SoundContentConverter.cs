using DevilDaggersInfo.App.Engine.Parsers.Sound;

namespace DevilDaggersInfo.App.Engine.Content.Conversion.Sounds;

internal sealed class SoundContentConverter : IContentConverter<SoundBinary>
{
	public static SoundBinary Construct(string inputPath)
	{
		SoundData soundData = WaveParser.Parse(File.ReadAllBytes(inputPath));
		return new(soundData.Channels, soundData.SampleRate, soundData.BitsPerSample, soundData.Data);
	}
}
