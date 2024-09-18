using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public class Texture
{
	private readonly GL _gl;

	public Texture(GL gl, uint id)
	{
		_gl = gl;
		Id = id;
	}

	public uint Id { get; }

	public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
	{
		_gl.ActiveTexture(textureUnit);
		_gl.BindTexture(TextureTarget.Texture2D, Id);
	}
}
