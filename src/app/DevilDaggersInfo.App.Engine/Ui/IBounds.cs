using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Utils;

namespace DevilDaggersInfo.App.Engine.Ui;

/// <summary>
/// Represents a "box" used for UI positioning. All values are in pixels.
/// </summary>
public interface IBounds
{
	int X1 { get; }
	int Y1 { get; }
	int X2 { get; }
	int Y2 { get; }

	public Vector2i<int> TopLeft => new(X1, Y1);
	public Vector2i<int> Size => new(X2 - X1, Y2 - Y1);
	public Vector2i<int> Center => new(X1 + (X2 - X1) / 2, Y1 + (Y2 - Y1) / 2);

	public bool Contains(int xInPixels, int yInPixels)
	{
		return xInPixels >= X1 && xInPixels <= X2 && yInPixels >= Y1 && yInPixels <= Y2;
	}

	public bool Contains(Vector2i<int> positionInPixels)
	{
		return Contains(positionInPixels.X, positionInPixels.Y);
	}

	public bool Overlaps(int x1InPixels, int y1InPixels, int x2InPixels, int y2InPixels)
	{
		BoundingRectangleUtils.Rectangle child = new(x1InPixels, y1InPixels, x2InPixels, y2InPixels);
		BoundingRectangleUtils.Rectangle parent = new(X1, Y1, X2, Y2);
		return BoundingRectangleUtils.Overlaps(child, parent);
	}

	IBounds CreateNested(int xInPixels, int yInPixels, int widthInPixels, int heightInPixels);

	Vector2 CreateNested(int xInPixels, int yInPixels);

	public (int X1, int Y1, int X2, int Y2) Move(int xInPixels, int yInPixels)
	{
		return (X1 + xInPixels, Y1 + yInPixels, X2 + xInPixels, Y2 + yInPixels);
	}
}
