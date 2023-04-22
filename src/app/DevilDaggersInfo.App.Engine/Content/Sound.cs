using Silk.NET.OpenAL;

namespace DevilDaggersInfo.App.Engine.Content;

public class Sound
{
	public Sound(short channels, int sampleRate, short bitsPerSample, int size, byte[] data)
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

	public unsafe uint CreateSource()
	{
		uint sourceId = Audio.Al.GenSource();
		uint bufferId = Audio.Al.GenBuffer();

		fixed (byte* b = &Data[0])
			Audio.Al.BufferData(bufferId, GetAudioFormat(), b, Data.Length, SampleRate);
		uint[] buffers = { bufferId };
		fixed (uint* u = &buffers[0])
			Audio.Al.SourceQueueBuffers(sourceId, 1, u);

		return sourceId;
	}

	private BufferFormat GetAudioFormat()
	{
		bool stereo = Channels > 1;

		return BitsPerSample switch
		{
			16 => stereo ? BufferFormat.Stereo16 : BufferFormat.Mono16,
			8 => stereo ? BufferFormat.Stereo8 : BufferFormat.Mono8,
			_ => throw new NotSupportedException($"Could not get audio format for wave with {BitsPerSample} samples."),
		};
	}
}
