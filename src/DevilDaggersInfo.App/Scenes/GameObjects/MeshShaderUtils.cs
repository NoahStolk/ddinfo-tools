using DevilDaggersInfo.App.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public static class MeshShaderUtils
{
	public static unsafe uint CreateVao(MeshContent mesh)
	{
		uint vao = Root.Gl.GenVertexArray();
		Root.Gl.BindVertexArray(vao);

		uint vbo = Root.Gl.GenBuffer();
		Root.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (Vertex* v = &mesh.Vertices[0])
			Root.Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		Root.Gl.EnableVertexAttribArray(0);
		Root.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		Root.Gl.EnableVertexAttribArray(1);
		Root.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		// TODO: We don't do anything with normals here.
		Root.Gl.EnableVertexAttribArray(2);
		Root.Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		Root.Gl.BindVertexArray(0);
		Root.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Root.Gl.DeleteBuffer(vbo);

		return vao;
	}
}
