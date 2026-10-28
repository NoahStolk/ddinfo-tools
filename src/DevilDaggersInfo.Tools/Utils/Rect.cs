using System.Numerics;

namespace DevilDaggersInfo.Tools.Utils;

internal readonly record struct Rect(float X, float Y, float W, float H)
{
	public Rect(Vector2 origin, Vector2 size)
		: this(origin.X, origin.Y, size.X, size.Y)
	{
	}

	public Vector2 Origin => new(X, Y);
}
