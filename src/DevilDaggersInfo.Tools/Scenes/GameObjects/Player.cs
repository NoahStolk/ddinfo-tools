using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public sealed class Player
{
	private static readonly Quaternion _rotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI);

	private static uint _vao;

	private readonly ReplaySimulation _movementTimeline;

	public Player(ReplaySimulation movementTimeline)
	{
		_movementTimeline = movementTimeline;
		Mesh = new PlayerMovement(_vao, ContentManager.Content.Hand4Mesh, default, default);
		Light = new LightObject(6, default, new Vector3(1, 0.5f, 0));
	}

	public PlayerMovement Mesh { get; }

	public LightObject Light { get; }

	public static void InitializeRendering(GL gl)
	{
		if (_vao != 0)
			throw new InvalidOperationException("Player is already initialized.");

		_vao = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Hand4Mesh);
	}

	public void Update(int currentTick)
	{
		const float offsetY = 3.3f;

		PlayerMovementSnapshot snapshot = _movementTimeline.GetPlayerMovementSnapshot(currentTick);
		Mesh.Rotation = snapshot.Rotation * _rotationOffset;
		Mesh.Position = snapshot.Position + new Vector3(0, offsetY, 0);

		Light.Position = Mesh.Position;
	}
}
