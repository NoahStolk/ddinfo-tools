using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public class Texture
{
	public Texture(uint id)
	{
		Id = id;
	}

	public uint Id { get; }

	public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
	{
		Graphics.Gl.ActiveTexture(textureUnit);
		Graphics.Gl.BindTexture(TextureTarget.Texture2D, Id);
	}
}
