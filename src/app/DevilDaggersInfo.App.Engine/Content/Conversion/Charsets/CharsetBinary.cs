namespace DevilDaggersInfo.App.Engine.Content.Conversion.Charsets;

internal record CharsetBinary(byte[] Characters) : IBinary<CharsetBinary>
{
	public ContentType ContentType => ContentType.Charset;

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write((ushort)Characters.Length);
		bw.Write(Characters);
		return ms.ToArray();
	}

	public static CharsetBinary FromStream(BinaryReader br)
	{
		ushort charactersLength = br.ReadUInt16();
		byte[] characters = br.ReadBytes(charactersLength);
		return new(characters);
	}
}
