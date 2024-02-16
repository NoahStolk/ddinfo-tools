using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public static class MeshShaderUtils
{
	public static unsafe uint CreateVao(MeshContent mesh)
	{
		uint vao = Graphics.Gl.GenVertexArray();
		Graphics.Gl.BindVertexArray(vao);

		uint vbo = Graphics.Gl.GenBuffer();
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (Vertex* v = &mesh.Vertices[0])
			Graphics.Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		// TODO: We don't do anything with normals here.
		Graphics.Gl.EnableVertexAttribArray(2);
		Graphics.Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		Graphics.Gl.BindVertexArray(0);
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Gl.DeleteBuffer(vbo);

		return vao;
	}
}
