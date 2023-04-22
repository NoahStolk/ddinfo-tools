using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Engine.Extensions;

public static class GenericVectorExtensions
{
	public static Vector2 ToVector2(this Vector2i<int> vector)
		=> new(vector.X, vector.Y);
}
