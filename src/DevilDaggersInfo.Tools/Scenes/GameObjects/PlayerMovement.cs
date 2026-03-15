using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class PlayerMovement(Quaternion rotation, Vector3 position)
{
	public Quaternion Rotation { get; set; } = rotation;
	public Vector3 Position { get; set; } = position;
}
