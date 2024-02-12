using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine.Loaders;

public static class ShaderLoader
{
	public static uint Load(string vertexCode, string fragmentCode)
	{
		uint vs = Graphics.Gl.CreateShader(ShaderType.VertexShader);
		Graphics.Gl.ShaderSource(vs, vertexCode);
		Graphics.Gl.CompileShader(vs);
		CheckShaderStatus(ShaderType.VertexShader, vs);

		uint fs = Graphics.Gl.CreateShader(ShaderType.FragmentShader);
		Graphics.Gl.ShaderSource(fs, fragmentCode);
		Graphics.Gl.CompileShader(fs);
		CheckShaderStatus(ShaderType.FragmentShader, fs);

		uint id = Graphics.Gl.CreateProgram();

		Graphics.Gl.AttachShader(id, vs);
		Graphics.Gl.AttachShader(id, fs);
		Graphics.Gl.LinkProgram(id);

		Graphics.Gl.DetachShader(id, vs);
		Graphics.Gl.DetachShader(id, fs);

		Graphics.Gl.DeleteShader(vs);
		Graphics.Gl.DeleteShader(fs);

		return id;
	}

	private static void CheckShaderStatus(ShaderType shaderType, uint shaderId)
	{
		string infoLog = Graphics.Gl.GetShaderInfoLog(shaderId);
		if (!string.IsNullOrWhiteSpace(infoLog))
			throw new InvalidOperationException($"{shaderType} compile error: {infoLog}");
	}
}
