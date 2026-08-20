using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public sealed class TextureLoader(GL gl)
{
	public unsafe uint Load(TextureContent texture)
	{
		uint textureId = gl.GenTexture();

		gl.BindTexture(TextureTarget.Texture2D, textureId);

		int textureWrapS = (int)GLEnum.ClampToEdge;
		int textureWrapT = (int)GLEnum.ClampToEdge;
		int textureMinFilter = (int)GLEnum.LinearMipmapLinear;
		int textureMagFilter = (int)GLEnum.Linear;
		int textureBaseLevel = 0;
		int textureMaxLevel = 8;
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, in textureWrapS);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, in textureWrapT);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, in textureMinFilter);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, in textureMagFilter);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, in textureBaseLevel);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, in textureMaxLevel);

		fixed (byte* b = texture.Pixels)
			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)texture.Width, (uint)texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
