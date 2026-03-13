using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public sealed class Texture(GL gl, uint id)
{
	public uint Id { get; } = id;

	public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
	{
		gl.ActiveTexture(textureUnit);
		gl.BindTexture(TextureTarget.Texture2D, Id);
	}
}
