using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public sealed class Texture(GL gl, uint id) : IDisposable
{
	private bool _disposed;

	public uint Id { get; } = id;

	public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
	{
		gl.ActiveTexture(textureUnit);
		gl.BindTexture(TextureTarget.Texture2D, Id);
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		gl.DeleteTexture(Id);
	}
}
