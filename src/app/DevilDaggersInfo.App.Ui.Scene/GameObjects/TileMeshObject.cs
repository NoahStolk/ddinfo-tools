using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Ui.Base;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Scene.GameObjects;

public class TileMeshObject
{
	private readonly uint _vao;
	private readonly Mesh _mesh;
	private readonly float _positionX;
	private readonly float _positionZ;

	public TileMeshObject(uint vao, Mesh mesh, float positionX, float positionZ)
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
		MeshShader.SetModel(translationMatrix);

		Gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Gl.BindVertexArray(0);
	}
}
