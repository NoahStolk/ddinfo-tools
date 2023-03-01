using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Utils;

public static class VertexArrayObjectUtils
{
	public static unsafe uint CreateFromVertices(float[] vertices)
	{
		uint vao = Gl.GenVertexArray();
		Gl.BindVertexArray(vao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (float* v = &vertices[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vertices.Length * sizeof(float), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);

		Gl.BindVertexArray(0);

		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

		return vao;
	}
}
