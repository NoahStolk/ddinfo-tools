using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class RaceDagger
{
	private const float _yOffset = 4;

	private static uint _vao;

	private static readonly Vector3 _hiddenPosition = new(-1000, -1000, -1000);

	private readonly Quaternion _meshRotationStart;

	private Vector3 _meshPosition;
	private Quaternion _meshRotation;

	public RaceDagger()
	{
		_meshPosition = default;
		_meshRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 0.5f);

		_meshRotationStart = _meshRotation;
	}

	public static void InitializeRendering()
	{
		if (_vao != 0)
			throw new InvalidOperationException("Race dagger is already initialized.");

		_vao = MeshShaderUtils.CreateVao(ContentManager.Content.DaggerMesh);
	}

	public void Update(SpawnsetBinary spawnset, int currentTick)
	{
		float currentTime = currentTick / 60f;
		float? raceDaggerHeight = spawnset.GetActualRaceDaggerHeight(currentTime);
		Vector3 basePosition = spawnset.GameMode != GameMode.Race || !raceDaggerHeight.HasValue ? _hiddenPosition : new()
		{
			X = spawnset.RaceDaggerPosition.X,
			Y = raceDaggerHeight.Value + _yOffset,
			Z = spawnset.RaceDaggerPosition.Y,
		};
		_meshPosition = basePosition + new Vector3(0, 0.15f + MathF.Sin(currentTime) * 0.15f, 0);
		_meshRotation = _meshRotationStart * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, currentTime);
	}

	public unsafe void Render()
	{
		Root.GameResources.DaggerSilverTexture.Bind();

		Root.InternalResources.MeshShader.SetUniform("model", Matrix4x4.CreateScale(8) * Matrix4x4.CreateFromQuaternion(_meshRotation) * Matrix4x4.CreateTranslation(_meshPosition));

		Root.Gl.BindVertexArray(_vao);
		fixed (uint* i = &ContentManager.Content.DaggerMesh.Indices[0])
			Root.Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.DaggerMesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		Root.Gl.BindVertexArray(0);
	}
}
