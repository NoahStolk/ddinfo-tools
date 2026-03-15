using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class Player(ReplaySimulation movementTimeline)
{
	private static readonly Quaternion _rotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI);

	public PlayerMovement PlayerMovement { get; } = new(default, default);

	public LightObject Light { get; } = new(6, default, new Vector3(1, 0.5f, 0));

	public void Update(int currentTick)
	{
		const float offsetY = 3.3f;

		PlayerMovementSnapshot snapshot = movementTimeline.GetPlayerMovementSnapshot(currentTick);
		PlayerMovement.Rotation = snapshot.Rotation * _rotationOffset;
		PlayerMovement.Position = snapshot.Position + new Vector3(0, offsetY, 0);

		Light.Position = PlayerMovement.Position;
	}
}
