using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Engine.ImGui;

internal class Shader
{
	private readonly Dictionary<string, int> _uniformToLocation = new();
	private readonly Dictionary<string, int> _attribLocation = new();
	private bool _initialized;
	private readonly GL _gl;

	public Shader(GL gl, string vertexShader, string fragmentShader)
	{
		_gl = gl;
		(ShaderType Type, string Path)[] files =
		{
			(ShaderType.VertexShader, vertexShader),
			(ShaderType.FragmentShader, fragmentShader),
		};
		Program = CreateProgram(files);
	}

	public uint Program { get; }

	public void UseShader()
	{
		_gl.UseProgram(Program);
	}

	public void Dispose()
	{
		if (_initialized)
		{
			_gl.DeleteProgram(Program);
			_initialized = false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetUniformLocation(string uniform)
	{
		if (!_uniformToLocation.TryGetValue(uniform, out int location))
		{
			location = _gl.GetUniformLocation(Program, uniform);
			_uniformToLocation.Add(uniform, location);

			if (location == -1)
			{
				Debug.Print($"The uniform '{uniform}' does not exist in the shader!");
			}
		}

		return location;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetAttribLocation(string attrib)
	{
		if (!_attribLocation.TryGetValue(attrib, out int location))
		{
			location = _gl.GetAttribLocation(Program, attrib);
			_attribLocation.Add(attrib, location);

			if (location == -1)
			{
				Debug.Print($"The attrib '{attrib}' does not exist in the shader!");
			}
		}

		return location;
	}

	private uint CreateProgram(params (ShaderType Type, string Source)[] shaderPaths)
	{
		uint program = _gl.CreateProgram();

		Span<uint> shaders = stackalloc uint[shaderPaths.Length];
		for (int i = 0; i < shaderPaths.Length; i++)
		{
			shaders[i] = CompileShader(shaderPaths[i].Type, shaderPaths[i].Source);
		}

		foreach (uint shader in shaders)
			_gl.AttachShader(program, shader);

		_gl.LinkProgram(program);

		_gl.GetProgram(program, GLEnum.LinkStatus, out int success);
		if (success == 0)
		{
			string info = _gl.GetProgramInfoLog(program);
			Debug.WriteLine($"GL.LinkProgram had info log:\n{info}");
		}

		foreach (uint shader in shaders)
		{
			_gl.DetachShader(program, shader);
			_gl.DeleteShader(shader);
		}

		_initialized = true;

		return program;
	}

	private uint CompileShader(ShaderType type, string source)
	{
		uint shader = _gl.CreateShader(type);
		_gl.ShaderSource(shader, source);
		_gl.CompileShader(shader);

		_gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
		if (success == 0)
		{
			string info = _gl.GetShaderInfoLog(shader);
			Debug.WriteLine($"GL.CompileShader for shader [{type}] had info log:\n{info}");
		}

		return shader;
	}
}
