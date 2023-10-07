using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Engine.ImGui;

public enum TextureCoordinate
{
	S = TextureParameterName.TextureWrapS,
	T = TextureParameterName.TextureWrapT,
	R = TextureParameterName.TextureWrapR,
}

internal class Texture : IDisposable
{
	public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)GLEnum.Srgb8Alpha8;

	public const GLEnum MaxTextureMaxAnisotropy = (GLEnum)0x84FF;

	public static float? MaxAniso;
	private readonly GL _gl;
	public readonly uint GlTexture;
	public readonly uint Width;
	public readonly uint Height;
	public readonly uint MipmapLevels;
	public readonly SizedInternalFormat InternalFormat;

	public unsafe Texture(GL gl, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
	{
		_gl = gl;
		MaxAniso ??= gl.GetFloat(MaxTextureMaxAnisotropy);
		Width = (uint)width;
		Height = (uint)height;
		InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
		MipmapLevels = (uint)(!generateMipmaps ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

		GlTexture = _gl.GenTexture();
		Bind();

		const PixelFormat pxFormat = PixelFormat.Bgra;

		_gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
		_gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, Width, Height, pxFormat, PixelType.UnsignedByte, (void*)data);

		if (generateMipmaps)
			_gl.GenerateTextureMipmap(GlTexture);
		SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
		SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

		_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
	}

	public void Bind()
	{
		_gl.BindTexture(GLEnum.Texture2D, GlTexture);
	}

	public void SetMinFilter(TextureMinFilter filter)
	{
		_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
	}

	public void SetMagFilter(TextureMagFilter filter)
	{
		_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);
	}

	public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
	{
		_gl.TexParameterI(GLEnum.Texture2D, (TextureParameterName)coord, (int)mode);
	}

	public void Dispose()
	{
		_gl.DeleteTexture(GlTexture);
	}
}
