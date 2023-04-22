namespace DevilDaggersInfo.App.Engine.Utils;

public static class VectorUtils
{
	public static Vector2 Clamp(Vector2 vector, float min, float max)
		=> new(Math.Clamp(vector.X, min, max), Math.Clamp(vector.Y, min, max));
}
