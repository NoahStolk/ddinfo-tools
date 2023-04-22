namespace DevilDaggersInfo.App.Engine.Utils;

internal static class BoundingRectangleUtils
{
	public static bool Overlaps(Rectangle a, Rectangle b)
	{
		bool xOverlap = ValueInRange(a.X1, b.X1, b.X2) || ValueInRange(b.X1, a.X1, a.X2);
		bool yOverlap = ValueInRange(a.Y1, b.Y1, b.Y2) || ValueInRange(b.Y1, a.Y1, a.Y2);
		return xOverlap && yOverlap;

		bool ValueInRange(int value, int min, int max) => value >= min && value <= max;
	}

	public readonly record struct Rectangle(int X1, int Y1, int X2, int Y2);
}
