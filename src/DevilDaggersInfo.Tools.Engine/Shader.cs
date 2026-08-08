using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public sealed class Shader(GL gl, uint id) : IDisposable
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	private bool _disposed;

	public uint Id { get; } = id;

	public int GetUniformLocation(string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		gl.DeleteProgram(Id);
	}
}
