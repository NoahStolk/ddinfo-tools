namespace DevilDaggersInfo.Tools.Engine.Content;

public class SoundContent
{
	public SoundContent(short channels, int sampleRate, short bitsPerSample, int size, byte[] data)
	{
		Channels = channels;
		SampleRate = sampleRate;
		BitsPerSample = bitsPerSample;
		Size = size;
		Data = data;
	}

	public short Channels { get; }
	public int SampleRate { get; }
	public short BitsPerSample { get; }
	public int Size { get; }
	public byte[] Data { get; }
}
