using Silk.NET.Maths;

namespace DevilDaggersInfo.App.Engine.Extensions;

public static class VectorExtensions
{
	public static Vector2D<int> FloorToVector2Int32(this Vector2 vector)
		=> new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));
}
