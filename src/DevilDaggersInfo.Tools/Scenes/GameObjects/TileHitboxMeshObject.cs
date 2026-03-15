using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class TileHitboxMeshObject(uint vao, MeshContent mesh, float positionX, float positionZ)
{
	private Matrix4x4 _model;

	public float PositionY
	{
		get;
		set
		{
			field = value;
			SetModel();
		}
	}

	public float Height
	{
		get;
		set
		{
			field = value;
			SetModel();
		}
	}

	private void SetModel()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, Height, 1));
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(positionX, PositionY, positionZ));
		_model = scaleMatrix * translationMatrix;
	}

	public unsafe void Render(GL gl, ResourceManager resourceManager)
	{
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), _model);

		gl.BindVertexArray(vao);
		fixed (uint* i = &mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		gl.BindVertexArray(0);
	}
}
