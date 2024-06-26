using DevilDaggersInfo.Tools.Engine.Content.Parsers.Sound;

namespace DevilDaggersInfo.Tools.Engine.Content.Conversion.Sounds;

internal sealed class SoundContentConverter : IContentConverter<SoundBinary>
{
	public static SoundBinary Construct(string inputPath)
	{
		SoundData soundData = WaveParser.Parse(File.ReadAllBytes(inputPath));
		return new SoundBinary(soundData.Channels, soundData.SampleRate, soundData.BitsPerSample, soundData.Data);
	}
}
