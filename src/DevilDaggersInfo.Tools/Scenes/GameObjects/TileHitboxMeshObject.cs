using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public class TileHitboxMeshObject
{
	private readonly uint _vao;
	private readonly MeshContent _mesh;
	private readonly float _positionX;
	private readonly float _positionZ;

	private Matrix4x4 _model;
	private float _positionY;
	private float _height;

	public TileHitboxMeshObject(uint vao, MeshContent mesh, float positionX, float positionZ)
	{
		_vao = vao;
		_mesh = mesh;
		_positionX = positionX;
		_positionZ = positionZ;
	}

	public float PositionY
	{
		get => _positionY;
		set
		{
			_positionY = value;
			SetModel();
		}
	}

	public float Height
	{
		get => _height;
		set
		{
			_height = value;
			SetModel();
		}
	}

	private void SetModel()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, Height, 1));
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(_positionX, PositionY, _positionZ));
		_model = scaleMatrix * translationMatrix;
	}

	public unsafe void Render(GL gl, ResourceManager resourceManager)
	{
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), _model);

		gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		gl.BindVertexArray(0);
	}
}
