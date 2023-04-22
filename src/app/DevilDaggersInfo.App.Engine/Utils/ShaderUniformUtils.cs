using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Engine.Utils;

/// <summary>
/// Provides various methods used by the generated code to set shader uniforms.
/// </summary>
public static class ShaderUniformUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, bool value)
	{
		Set(uniformLocation, value ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, int value)
	{
		Gl.Gl.Uniform1(uniformLocation, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, uint value)
	{
		Gl.Gl.Uniform1(uniformLocation, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, float value)
	{
		Gl.Gl.Uniform1(uniformLocation, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, Vector2 value)
	{
		Gl.Gl.Uniform2(uniformLocation, value.X, value.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, Vector3 value)
	{
		Gl.Gl.Uniform3(uniformLocation, value.X, value.Y, value.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Set(int uniformLocation, Vector4 value)
	{
		Gl.Gl.Uniform4(uniformLocation, value.X, value.Y, value.Z, value.W);
	}

	public static void Set(int uniformLocation, Matrix4x4 value)
	{
		Span<float> data = stackalloc float[16]
		{
			value.M11, value.M12, value.M13, value.M14,
			value.M21, value.M22, value.M23, value.M24,
			value.M31, value.M32, value.M33, value.M34,
			value.M41, value.M42, value.M43, value.M44,
		};
		Gl.Gl.UniformMatrix4(uniformLocation, 1, false, data);
	}

	public static void Set(int uniformLocation, ReadOnlySpan<float> values)
	{
		if (values.IsEmpty)
			return;

		Gl.Gl.Uniform1(uniformLocation, values);
	}

	public static void Set(int uniformLocation, ReadOnlySpan<Vector3> values)
	{
		if (values.IsEmpty)
			return;

		Span<float> array = stackalloc float[values.Length * 3];
		for (int i = 0; i < values.Length; i++)
		{
			Vector3 vector = values[i];
			array[i * 3] = vector.X;
			array[i * 3 + 1] = vector.Y;
			array[i * 3 + 2] = vector.Z;
		}

		Gl.Gl.Uniform3(uniformLocation, array);
	}
}
