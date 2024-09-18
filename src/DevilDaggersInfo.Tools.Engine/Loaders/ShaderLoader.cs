using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public sealed class ShaderLoader
{
	private readonly GL _gl;

	public ShaderLoader(GL gl)
	{
		_gl = gl;
	}

	public uint Load(string vertexCode, string fragmentCode)
	{
		uint vs = _gl.CreateShader(ShaderType.VertexShader);
		_gl.ShaderSource(vs, vertexCode);
		_gl.CompileShader(vs);
		CheckShaderStatus(ShaderType.VertexShader, vs);

		uint fs = _gl.CreateShader(ShaderType.FragmentShader);
		_gl.ShaderSource(fs, fragmentCode);
		_gl.CompileShader(fs);
		CheckShaderStatus(ShaderType.FragmentShader, fs);

		uint id = _gl.CreateProgram();

		_gl.AttachShader(id, vs);
		_gl.AttachShader(id, fs);
		_gl.LinkProgram(id);

		_gl.DetachShader(id, vs);
		_gl.DetachShader(id, fs);

		_gl.DeleteShader(vs);
		_gl.DeleteShader(fs);

		return id;
	}

	private void CheckShaderStatus(ShaderType shaderType, uint shaderId)
	{
		string infoLog = _gl.GetShaderInfoLog(shaderId);
		if (!string.IsNullOrWhiteSpace(infoLog))
			throw new InvalidOperationException($"{shaderType} compile error: {infoLog}");
	}
}
