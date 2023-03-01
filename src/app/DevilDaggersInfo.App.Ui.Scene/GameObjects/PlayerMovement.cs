using DevilDaggersInfo.App.Ui.Base;
using Silk.NET.OpenGL;
using Warp.NET.Content;
using Warp.NET.InterpolationStates;

namespace DevilDaggersInfo.App.Ui.Scene.GameObjects;

public class PlayerMovement
{
	private readonly uint _vao;
	private readonly Mesh _mesh;

	public PlayerMovement(uint vao, Mesh mesh, Quaternion rotation, Vector3 position)
	{
		_vao = vao;
		_mesh = mesh;

		RotationState = new(rotation);
		PositionState = new(position);
	}

	public QuaternionState RotationState { get; }

	public Vector3State PositionState { get; }

	public void PrepareUpdate()
	{
		RotationState.PrepareUpdate();
		PositionState.PrepareUpdate();
	}

	public unsafe void Render()
	{
		RotationState.PrepareRender();
		PositionState.PrepareRender();

		Shader.SetMatrix4x4(MeshUniforms.Model, Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(RotationState.Render) * Matrix4x4.CreateTranslation(PositionState.Render));

		Gl.BindVertexArray(_vao);
		fixed (uint* i = &_mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Gl.BindVertexArray(0);
	}
}
