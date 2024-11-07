using DevilDaggersInfo.Tools.Engine.Content;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public class PlayerMovement
{
	public PlayerMovement(uint vao, MeshContent mesh, Quaternion rotation, Vector3 position)
	{
		Vao = vao;
		Mesh = mesh;

		Rotation = rotation;
		Position = position;
	}

	public uint Vao { get; }
	public MeshContent Mesh { get; }

	public Quaternion Rotation { get; set; }
	public Vector3 Position { get; set; }
}
