using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public sealed class Shader
{
	private readonly GL _gl;
	private readonly Dictionary<string, int> _uniformLocations = new();

	public Shader(GL gl, uint id)
	{
		_gl = gl;
		Id = id;
	}

	public uint Id { get; }

	public int GetUniformLocation(string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = _gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
