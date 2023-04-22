using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Ui.Base;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Scene.GameObjects;

public class Skull4
{
	private static uint _vaoMain;
	private static uint _vaoJaw;

	public static unsafe void Initialize()
	{
		_vaoMain = CreateVao(ContentManager.Content.Skull4Mesh);
		_vaoJaw = CreateVao(ContentManager.Content.Skull4JawMesh);

		static uint CreateVao(Mesh mesh)
		{
			uint vao = Gl.GenVertexArray();
			Gl.BindVertexArray(vao);

			uint vbo = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

			fixed (Vertex* v = &mesh.Vertices[0])
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

			Gl.EnableVertexAttribArray(0);
			Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

			Gl.EnableVertexAttribArray(1);
			Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

			// TODO: We don't do anything with normals here.
			Gl.EnableVertexAttribArray(2);
			Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

			Gl.BindVertexArray(0);
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			Gl.DeleteBuffer(vbo);

			return vao;
		}
	}

	public unsafe void Render()
	{
		MeshShader.SetModel(Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new(0, 4f, 0)));

		ContentManager.Content.Skull4Texture.Use();

		Gl.BindVertexArray(_vaoMain);
		fixed (uint* i = &ContentManager.Content.Skull4Mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		ContentManager.Content.Skull4JawTexture.Use();

		Gl.BindVertexArray(_vaoJaw);
		fixed (uint* i = &ContentManager.Content.Skull4JawMesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4JawMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		Gl.BindVertexArray(0);
	}
}
