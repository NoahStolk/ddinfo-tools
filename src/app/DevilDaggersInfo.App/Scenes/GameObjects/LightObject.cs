using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class LightObject
{
	public LightObject(float radius, Vector3 position, Vector3 color)
	{
		Radius = radius;
		Position = position;
		Color = color;
	}

	public float Radius { get; set; }
	public Vector3 Position { get; set; }
	public Vector3 Color { get; set; }
}
