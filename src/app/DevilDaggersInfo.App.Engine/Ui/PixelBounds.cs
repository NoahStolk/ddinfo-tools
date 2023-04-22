namespace DevilDaggersInfo.App.Engine.Ui;

/// <summary>
/// An implementation of <see cref="IBounds"/> which uses pixel coordinates. This results in pixel-perfect rendering.
/// </summary>
/// <param name="X">The X coordinate in pixels.</param>
/// <param name="Y">The Y coordinate in pixels.</param>
/// <param name="Width">The width in pixels.</param>
/// <param name="Height">The height in pixels.</param>
public sealed record PixelBounds(int X, int Y, int Width, int Height) : IBounds
{
	public int X1 => X;
	public int Y1 => Y;
	public int X2 => X + Width;
	public int Y2 => Y + Height;

	public IBounds CreateNested(int xInPixels, int yInPixels, int widthInPixels, int heightInPixels)
	{
		return new PixelBounds(X + xInPixels, Y + yInPixels, widthInPixels, heightInPixels);
	}

	public Vector2 CreateNested(int xInPixels, int yInPixels)
	{
		return new(X + xInPixels, Y + yInPixels);
	}
}
