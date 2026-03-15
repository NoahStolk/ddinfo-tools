using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class LightObject(float radius, Vector3 position, Vector3 color)
{
	public float Radius { get; set; } = radius;
	public Vector3 Position { get; set; } = position;
	public Vector3 Color { get; set; } = color;
}
