using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Engine.Content;

public class Shader
{
	public Shader(string vertexCode, string? geometryCode, string fragmentCode)
	{
		uint vs = Gl.Gl.CreateShader(ShaderType.VertexShader);
		Gl.Gl.ShaderSource(vs, vertexCode);
		Gl.Gl.CompileShader(vs);
		CheckShaderStatus(vs);

		uint? gs = null;
		if (geometryCode != null)
		{
			gs = Gl.Gl.CreateShader(ShaderType.GeometryShader);
			Gl.Gl.ShaderSource(gs.Value, geometryCode);
			Gl.Gl.CompileShader(gs.Value);
			CheckShaderStatus(gs.Value);
		}

		uint fs = Gl.Gl.CreateShader(ShaderType.FragmentShader);
		Gl.Gl.ShaderSource(fs, fragmentCode);
		Gl.Gl.CompileShader(fs);
		CheckShaderStatus(fs);

		Id = Gl.Gl.CreateProgram();

		Gl.Gl.AttachShader(Id, vs);
		if (gs.HasValue)
			Gl.Gl.AttachShader(Id, gs.Value);
		Gl.Gl.AttachShader(Id, fs);
		Gl.Gl.LinkProgram(Id);

		Gl.Gl.DetachShader(Id, vs);
		if (gs.HasValue)
			Gl.Gl.DetachShader(Id, gs.Value);
		Gl.Gl.DetachShader(Id, fs);

		Gl.Gl.DeleteShader(vs);
		if (gs.HasValue)
			Gl.Gl.DeleteShader(gs.Value);
		Gl.Gl.DeleteShader(fs);
	}

	public uint Id { get; }

	private static void CheckShaderStatus(uint shaderId)
	{
		string infoLog = Gl.Gl.GetShaderInfoLog(shaderId);
		if (!string.IsNullOrWhiteSpace(infoLog))
			throw new InvalidOperationException($"Shader compile error: {infoLog}");
	}
}
