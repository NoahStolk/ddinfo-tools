using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class GlExtensions
{
	// ReSharper disable once InconsistentNaming
	public static unsafe void UniformMatrix4x4(this GL gl, int uniformLocation, Matrix4x4 matrix)
	{
		gl.UniformMatrix4(uniformLocation, 1, false, (float*)&matrix);
	}
}
