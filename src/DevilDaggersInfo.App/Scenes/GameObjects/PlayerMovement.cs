using DevilDaggersInfo.App.Engine.Content;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class PlayerMovement
{
	private readonly uint _vao;
	private readonly MeshContent _mesh;

	public PlayerMovement(uint vao, MeshContent mesh, Quaternion rotation, Vector3 position)
	{
		_vao = vao;
		_mesh = mesh;

		Rotation = rotation;
		Position = position;
	}

	public Quaternion Rotation { get; set; }
	public Vector3 Position { get; set; }

	public unsafe void Render()
	{
		Root.InternalResources.MeshShader.SetUniform("model", Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position));

		Root.Gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			Root.Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Root.Gl.BindVertexArray(0);
	}
}
