using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

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
		Graphics.Gl.UniformMatrix4x4(Root.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position));

		Graphics.Gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			Graphics.Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Graphics.Gl.BindVertexArray(0);
	}
}
