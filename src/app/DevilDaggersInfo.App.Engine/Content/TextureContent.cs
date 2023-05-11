namespace DevilDaggersInfo.App.Engine.Content;

public class TextureContent
{
	public TextureContent(int width, int height, byte[] pixels)
	{
		int expectedNumberOfPixels = width * height * 4;
		if (pixels.Length != expectedNumberOfPixels)
			throw new ArgumentException($"Incorrect number of pixels {pixels.Length} for RGBA texture with size {width}x{height}. Expected number of pixels is {expectedNumberOfPixels}.", nameof(pixels));

		Width = width;
		Height = height;
		Pixels = pixels;
	}

	public int Width { get; }
	public int Height { get; }

	public byte[] Pixels { get; }
}
