namespace DevilDaggersInfo.App.Engine.Content.Conversion.Sounds;

internal record SoundBinary(short Channels, int SampleRate, short BitsPerSample, byte[] Data) : IBinary<SoundBinary>
{
	public ContentType ContentType => ContentType.Sound;

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write(Channels);
		bw.Write(SampleRate);
		bw.Write(BitsPerSample);
		bw.Write(Data.Length);
		bw.Write(Data);
		return ms.ToArray();
	}

	public static SoundBinary FromStream(BinaryReader br)
	{
		short channels = br.ReadInt16();
		int sampleRate = br.ReadInt32();
		short bitsPerSample = br.ReadInt16();
		int size = br.ReadInt32();
		byte[] data = br.ReadBytes(size);
		return new(channels, sampleRate, bitsPerSample, data);
	}
}
