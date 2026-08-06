using DevilDaggersInfo.Tools.Engine;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class PlayerMovement(GpuMesh mesh, Quaternion rotation, Vector3 position)
{
	public GpuMesh Mesh { get; } = mesh;

	public Quaternion Rotation { get; set; } = rotation;
	public Vector3 Position { get; set; } = position;
}
