using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public static class MeshShaderUtils
{
	public static unsafe uint CreateVao(GL gl, MeshContent mesh)
	{
		uint vao = gl.GenVertexArray();
		gl.BindVertexArray(vao);

		uint vbo = gl.GenBuffer();
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (Vertex* v = &mesh.Vertices[0])
			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		gl.EnableVertexAttribArray(1);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		// TODO: We don't do anything with normals here.
		gl.EnableVertexAttribArray(2);
		gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		gl.BindVertexArray(0);
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		gl.DeleteBuffer(vbo);

		return vao;
	}
}
