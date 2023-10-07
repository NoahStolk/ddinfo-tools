namespace DevilDaggersInfo.App.Engine.Content.Conversion.Blobs;

internal record BlobBinary(byte[] Data) : IBinary<BlobBinary>
{
	public ContentType ContentType => ContentType.Blob;

	public byte[] ToBytes()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write(Data.Length);
		bw.Write(Data);
		return ms.ToArray();
	}

	public static BlobBinary FromStream(BinaryReader br)
	{
		int dataLength = br.ReadInt32();
		byte[] data = br.ReadBytes(dataLength);
		return new(data);
	}
}
