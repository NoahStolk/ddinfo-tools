namespace DevilDaggersInfo.App.Engine.Parsers.Sound;

/// <summary>
/// Represents data parsed from a sound format, such as a .wav file.
/// </summary>
public record SoundData(short Channels, int SampleRate, int ByteRate, short BlockAlign, short BitsPerSample, byte[] Data, int SampleCount, double LengthInSeconds);
