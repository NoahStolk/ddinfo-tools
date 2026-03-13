using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

public sealed class Shader(GL gl, uint id)
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	public uint Id { get; } = id;

	public int GetUniformLocation(string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
