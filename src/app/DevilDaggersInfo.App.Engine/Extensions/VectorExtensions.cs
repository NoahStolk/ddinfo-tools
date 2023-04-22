using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Engine.Extensions;

public static class VectorExtensions
{
	public static Vector2i<int> FloorToVector2Int32(this Vector2 vector)
		=> new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));

	public static Vector2i<int> RoundToVector2Int32(this Vector2 vector)
		=> new((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));
}
