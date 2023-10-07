using DevilDaggersInfo.App.Engine.Content;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class TileMeshObject
{
	private readonly uint _vao;
	private readonly MeshContent _mesh;
	private readonly float _positionX;
	private readonly float _positionZ;

	public TileMeshObject(uint vao, MeshContent mesh, float positionX, float positionZ)
	{
		_vao = vao;
		_mesh = mesh;
		_positionX = positionX;
		_positionZ = positionZ;
	}

	public float PositionY { get; set; }

	public unsafe void Render()
	{
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new(_positionX, PositionY, _positionZ));
		Root.InternalResources.MeshShader.SetUniform("model", translationMatrix);

		Root.Gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			Root.Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Root.Gl.BindVertexArray(0);
	}
}
