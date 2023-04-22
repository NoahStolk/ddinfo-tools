using DevilDaggersInfo.App.ContentSourceGen.Utils;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Shader;

public sealed record ShaderUniform
{
	public ShaderUniform(string type, string name)
	{
		Name = name;

		string cSharpType = GetCSharpType(type) ?? "System.Int32";
		string fieldName = SourceBuilderUtils.ToField(name);
		string methodName = SourceBuilderUtils.ToMethod(name);

		LocationField = $"private static int {fieldName} = -1;";
		LocationFieldInitializer = $"{fieldName} = {Constants.RootNamespace}.Graphics.Gl.GetUniformLocation(_shaderId, \"{name}\");";

		// TODO: Use "in" keyword for readonly parameters.
		// TODO: Find a better way to do the indentation here.
		UniformMethod = $$"""
				public static void Set{{methodName}}({{cSharpType}} value)
				{
					{{Constants.RootNamespace}}.Utils.ShaderUniformUtils.Set({{fieldName}}, value);
				}

			""";
	}

	public string Name { get; }

	public string LocationField { get; }

	public string LocationFieldInitializer { get; }

	public string UniformMethod { get; }

	/// <summary>
	/// See <a href="https://www.khronos.org/opengl/wiki/Data_Type_(GLSL)">Data Type (GLSL)</a>.
	/// </summary>
	private static string? GetCSharpType(string glslType)
	{
		return glslType switch
		{
			"bool" => "System.Boolean",
			"int" => "System.Int32",
			"uint" => "System.UInt32",
			"float" => "System.Single",
			"double" => "System.Double",

			"bvec2" => $"{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.Boolean>",
			"bvec3" => null,
			"bvec4" => null,

			"ivec2" => $"{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.Int32>",
			"ivec3" => null,
			"ivec4" => null,

			"uvec2" => $"{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.UInt32>",
			"uvec3" => null,
			"uvec4" => null,

			"vec2" => "System.Numerics.Vector2",
			"vec3" => "System.Numerics.Vector3",
			"vec4" => "System.Numerics.Vector4",

			"dvec2" => null,
			"dvec3" => null,
			"dvec4" => null,

			"mat2" or "mat2x2" => null,
			"mat2x3" => null,
			"mat2x4" => null,

			"mat3x2" => "System.Numerics.Matrix3x2",
			"mat3" or "mat3x3" => null,
			"mat3x4" => null,

			"mat4x2" => null,
			"mat4x3" => null,
			"mat4" or "mat4x4" => "System.Numerics.Matrix4x4",

			"bool[]" => "System.ReadOnlySpan<System.Boolean>",
			"int[]" => "System.ReadOnlySpan<System.Int32>",
			"uint[]" => "System.ReadOnlySpan<System.UInt32>",
			"float[]" => "System.ReadOnlySpan<System.Single>",
			"double[]" => "System.ReadOnlySpan<System.Double>",

			"bvec2[]" => $"System.ReadOnlySpan<{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.Boolean>>",
			"bvec3[]" => null,
			"bvec4[]" => null,

			"ivec2[]" => $"System.ReadOnlySpan<{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.Int32>>",
			"ivec3[]" => null,
			"ivec4[]" => null,

			"uvec2[]" => $"System.ReadOnlySpan<{Constants.RootNamespace}.Maths.Numerics.Vector2i<System.UInt32>>",
			"uvec3[]" => null,
			"uvec4[]" => null,

			"vec2[]" => "System.ReadOnlySpan<System.Numerics.Vector2>",
			"vec3[]" => "System.ReadOnlySpan<System.Numerics.Vector3>",
			"vec4[]" => "System.ReadOnlySpan<System.Numerics.Vector4>",

			"dvec2[]" => null,
			"dvec3[]" => null,
			"dvec4[]" => null,

			"mat2[]" or "mat2x2[]" => null,
			"mat2x3[]" => null,
			"mat2x4[]" => null,

			"mat3x2[]" => "System.ReadOnlySpan<System.Numerics.Matrix3x2>",
			"mat3[]" or "mat3x3[]" => null,
			"mat3x4[]" => null,

			"mat4x2[]" => null,
			"mat4x3[]" => null,
			"mat4[]" or "mat4x4[]" => "System.ReadOnlySpan<System.Numerics.Matrix4x4>",

			_ => null,
		};
	}
}
