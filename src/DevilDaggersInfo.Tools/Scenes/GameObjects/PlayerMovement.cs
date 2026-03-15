using DevilDaggersInfo.Tools.Engine.Content;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class PlayerMovement(uint vao, MeshContent mesh, Quaternion rotation, Vector3 position)
{
	public uint Vao { get; } = vao;
	public MeshContent Mesh { get; } = mesh;

	public Quaternion Rotation { get; set; } = rotation;
	public Vector3 Position { get; set; } = position;
}
