namespace DevilDaggersInfo.App.Engine.Content;

public class BlobContent
{
	public BlobContent(byte[] data)
	{
		Data = data;
	}

	public byte[] Data { get; }
}
