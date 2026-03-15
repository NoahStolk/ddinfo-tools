using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class Player(ReplaySimulation movementTimeline)
{
	private static readonly Quaternion _rotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI);

	private static uint _vao;

	public PlayerMovement Mesh { get; } = new(_vao, ContentManager.Content.Hand4Mesh, default, default);

	public LightObject Light { get; } = new(6, default, new Vector3(1, 0.5f, 0));

	public static void InitializeRendering(GL gl)
	{
		if (_vao != 0)
			throw new InvalidOperationException("Player is already initialized.");

		_vao = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Hand4Mesh);
	}

	public void Update(int currentTick)
	{
		const float offsetY = 3.3f;

		PlayerMovementSnapshot snapshot = movementTimeline.GetPlayerMovementSnapshot(currentTick);
		Mesh.Rotation = snapshot.Rotation * _rotationOffset;
		Mesh.Position = snapshot.Position + new Vector3(0, offsetY, 0);

		Light.Position = Mesh.Position;
	}
}
