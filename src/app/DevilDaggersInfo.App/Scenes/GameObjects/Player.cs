using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class Player
{
	private static readonly Quaternion _rotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI);

	private static uint _vao;

	private readonly ReplaySimulation _movementTimeline;
	private readonly PlayerMovement _mesh;

	public Player(ReplaySimulation movementTimeline)
	{
		_movementTimeline = movementTimeline;
		_mesh = new(_vao, ContentManager.Content.Hand4Mesh, default, default);
		Light = new(6, default, new(1, 0.5f, 0));
	}

	public LightObject Light { get; }

	public static void InitializeRendering()
	{
		if (_vao != 0)
			throw new InvalidOperationException("Player is already initialized.");

		_vao = MeshShaderUtils.CreateVao(ContentManager.Content.Hand4Mesh);
	}

	public void Update(int currentTick)
	{
		const float offsetY = 3.3f;

		PlayerMovementSnapshot snapshot = _movementTimeline.GetPlayerMovementSnapshot(currentTick);
		_mesh.Rotation = snapshot.Rotation * _rotationOffset;
		_mesh.Position = snapshot.Position + new Vector3(0, offsetY, 0);

		Light.Position = _mesh.Position;
	}

	public void Render()
	{
		Root.GameResources.Hand4Texture.Bind();
		_mesh.Render();
	}
}
