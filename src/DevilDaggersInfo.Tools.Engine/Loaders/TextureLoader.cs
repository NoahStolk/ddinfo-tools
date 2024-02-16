using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public static class TextureLoader
{
	public static unsafe uint Load(TextureContent texture)
	{
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

		fixed (byte* b = texture.Pixels)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)texture.Width, (uint)texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		Graphics.Gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
