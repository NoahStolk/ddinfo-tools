using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Engine;

public class Texture : IDisposable
{
	private readonly GL _gl;

	public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
	{
		_gl = gl;

		Handle = _gl.GenTexture();
		Bind();

		fixed (void* d = &data[0])
		{
			_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
			SetParameters();
		}
	}

	public uint Handle { get; }

	private void SetParameters()
	{
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
		_gl.GenerateMipmap(TextureTarget.Texture2D);
	}

	public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
	{
		_gl.ActiveTexture(textureSlot);
		_gl.BindTexture(TextureTarget.Texture2D, Handle);
	}

	public void Dispose()
	{
		_gl.DeleteTexture(Handle);
	}
}
