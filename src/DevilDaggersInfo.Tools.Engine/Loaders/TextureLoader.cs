using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public class TextureLoader
{
	private readonly GL _gl;

	public TextureLoader(GL gl)
	{
		_gl = gl;
	}

	public unsafe uint Load(TextureContent texture)
	{
		uint textureId = _gl.GenTexture();

		_gl.BindTexture(TextureTarget.Texture2D, textureId);

		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		_gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

		fixed (byte* b = texture.Pixels)
			_gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)texture.Width, (uint)texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		_gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
