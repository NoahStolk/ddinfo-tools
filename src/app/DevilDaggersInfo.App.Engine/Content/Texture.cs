using Silk.NET.OpenGL;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.Engine.Content;

public class Texture
{
	public unsafe Texture(int width, int height, byte[] pixels)
	{
		int expectedNumberOfPixels = width * height * 4;
		if (pixels.Length != expectedNumberOfPixels)
			throw new ArgumentException($"Incorrect number of pixels {pixels.Length} for RGBA texture with size {width}x{height}. Expected number of pixels is {expectedNumberOfPixels}.", nameof(pixels));

		Width = width;
		Height = height;
		Pixels = pixels.ToImmutableArray();

		Id = Gl.Gl.GenTexture();

		Gl.Gl.BindTexture(TextureTarget.Texture2D, Id);

		Gl.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Gl.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Gl.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		Gl.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = pixels)
			Gl.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)Width, (uint)Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		Gl.Gl.GenerateMipmap(TextureTarget.Texture2D);
	}

	public uint Id { get; }

	public int Width { get; }
	public int Height { get; }

	public ImmutableArray<byte> Pixels { get; }

	public void Use(TextureUnit unit = TextureUnit.Texture0)
	{
		Gl.Gl.ActiveTexture(unit);
		Gl.Gl.BindTexture(TextureTarget.Texture2D, Id);
	}
}
