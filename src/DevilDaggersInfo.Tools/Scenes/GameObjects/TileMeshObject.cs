using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

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

	public unsafe void Render(GL gl, ResourceManager resourceManager)
	{
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(_positionX, PositionY, _positionZ));
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), translationMatrix);

		gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		gl.BindVertexArray(0);
	}
}
