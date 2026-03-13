using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public sealed class TextureLoader(GL gl)
{
	public unsafe uint Load(TextureContent texture)
	{
		uint textureId = gl.GenTexture();

		gl.BindTexture(TextureTarget.Texture2D, textureId);

		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

		fixed (byte* b = texture.Pixels)
			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)texture.Width, (uint)texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
