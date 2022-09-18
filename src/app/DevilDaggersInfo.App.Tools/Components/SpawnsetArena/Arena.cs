using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.App.Tools.States;
using DevilDaggersInfo.App.Tools.Utils;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;
using Warp;
using Warp.Extensions;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetArena;

public class Arena : AbstractComponent
{
	private readonly int _tileSize;

	private Vector2i<int>? _pencilStart;
	private Vector2i<int>? _lineStart;
	private Vector2i<int>? _rectangleStart;

	public Arena(Vector2i<int> topLeft, int tileSize)
		: base(new(topLeft.X, topLeft.Y, topLeft.X + tileSize * SpawnsetBinary.ArenaDimensionMax, topLeft.Y + tileSize * SpawnsetBinary.ArenaDimensionMax))
	{
		_tileSize = tileSize;
	}

	public override void Update(Vector2i<int> parentPosition)
	{
		base.Update(parentPosition);

		bool hover = MouseUiContext.Contains(parentPosition, Metric);
		if (!hover)
		{
			Reset();
			return;
		}

		int relMouseX = (int)MouseUiContext.MousePosition.X - Metric.X1 - parentPosition.X;
		int relMouseY = (int)MouseUiContext.MousePosition.Y - Metric.Y1 - parentPosition.Y;
		int x = relMouseX / _tileSize;
		int y = relMouseY / _tileSize;
		if (x < 0 || y < 0 || x >= StateManager.SpawnsetState.Spawnset.ArenaDimension || y >= StateManager.SpawnsetState.Spawnset.ArenaDimension)
		{
			Reset();
			return;
		}

		Base.Game.CursorText = $"{StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y]}\n{{{x}, {y}}}";
		int scroll = Input.GetScroll();
		if (scroll != 0)
		{
			// TODO: Selection.
			StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y] += scroll;
			return;
		}

		switch (StateManager.ArenaEditorState.ArenaTool)
		{
			case ArenaTool.Pencil:
				if (Input.IsButtonHeld(MouseButton.Left))
				{
					if (_pencilStart.HasValue)
					{
						Vector2i<int> pencilEnd = new(relMouseX, relMouseY);
						Rectangle rectangle = GetRectangle(_pencilStart.Value / _tileSize, pencilEnd / _tileSize);
						for (int i = rectangle.X1; i <= rectangle.X2; i++)
						{
							for (int j = rectangle.Y1; j <= rectangle.Y2; j++)
							{
								Vector2 visualTileCenter = new Vector2(i, j) * _tileSize + new Vector2(_tileSize / 2f);
								if (LineIntersectsSquare(_pencilStart.Value.ToVector2(), pencilEnd.ToVector2(), visualTileCenter, _tileSize))
									StateManager.SpawnsetState.Spawnset.ArenaTiles[i, j] = StateManager.ArenaEditorState.SelectedHeight;
							}
						}

						StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y] = StateManager.ArenaEditorState.SelectedHeight;
					}

					_pencilStart = new(relMouseX, relMouseY);
				}
				else if (Input.IsButtonReleased(MouseButton.Left))
				{
					_pencilStart = null;
					SpawnsetHistoryManager.Save("Arena pencil edit");
				}
				break;
			case ArenaTool.Line:
				if (Input.IsButtonPressed(MouseButton.Left))
				{
					_lineStart = new(relMouseX, relMouseY);
				}
				else if (Input.IsButtonReleased(MouseButton.Left))
				{
					if (_lineStart.HasValue)
					{
						Vector2i<int> lineEnd = new(relMouseX, relMouseY);
						Rectangle rectangle = GetRectangle(_lineStart.Value / _tileSize, lineEnd / _tileSize);
						for (int i = rectangle.X1; i <= rectangle.X2; i++)
						{
							for (int j = rectangle.Y1; j <= rectangle.Y2; j++)
							{
								Vector2 visualTileCenter = new Vector2(i, j) * _tileSize + new Vector2(_tileSize / 2f);
								if (LineIntersectsSquare(_lineStart.Value.ToVector2(), lineEnd.ToVector2(), visualTileCenter, _tileSize))
									StateManager.SpawnsetState.Spawnset.ArenaTiles[i, j] = StateManager.ArenaEditorState.SelectedHeight;
							}
						}

						StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y] = StateManager.ArenaEditorState.SelectedHeight;
					}

					_lineStart = null;
					SpawnsetHistoryManager.Save("Arena line edit");
				}
				break;
			case ArenaTool.Rectangle:
				if (Input.IsButtonPressed(MouseButton.Left))
				{
					_rectangleStart = new(x, y);
				}
				else if (Input.IsButtonReleased(MouseButton.Left))
				{
					if (_rectangleStart.HasValue)
					{
						Vector2i<int> rectangleEnd = new(x, y);
						Rectangle rectangle = GetRectangle(_rectangleStart.Value, rectangleEnd);
						for (int i = rectangle.X1; i <= rectangle.X2; i++)
						{
							for (int j = rectangle.Y1; j <= rectangle.Y2; j++)
								StateManager.SpawnsetState.Spawnset.ArenaTiles[i, j] = StateManager.ArenaEditorState.SelectedHeight;
						}
					}

					SpawnsetHistoryManager.Save("Arena rectangle edit");
					_rectangleStart = null;
				}
				break;
			case ArenaTool.Bucket:
				if (Input.IsButtonPressed(MouseButton.Left))
				{
					List<Vector2i<int>> done = new();
					float[,] tiles = StateManager.SpawnsetState.Spawnset.ArenaTiles;
					int dimension = StateManager.SpawnsetState.Spawnset.ArenaDimension;
					float targetHeight = StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y];
					FillNeighbors(x, y);

					SpawnsetHistoryManager.Save("Arena bucket edit");

					void FillNeighbors(int x, int y)
					{
						const float tolerance = 0.1f;
						StateManager.SpawnsetState.Spawnset.ArenaTiles[x, y] = StateManager.ArenaEditorState.SelectedHeight;
						done.Add(new(x, y));

						int leftX = x - 1;
						int rightX = x + 1;
						int topY = y - 1;
						int bottomY = y + 1;

						if (leftX >= 0)
							FillIfApplicable(leftX, y);
						if (rightX < dimension)
							FillIfApplicable(rightX, y);
						if (topY >= 0)
							FillIfApplicable(x, topY);
						if (bottomY < dimension)
							FillIfApplicable(x, bottomY);

						void FillIfApplicable(int newX, int newY)
						{
							if (!done.Contains(new(newX, newY)) && MathF.Abs(tiles[newX, newY] - targetHeight) < tolerance)
								FillNeighbors(newX, newY);
						}
					}
				}
				break;
		}
	}

	private void Reset()
	{
		_pencilStart = null;
		_lineStart = null;
		_rectangleStart = null;
	}

	private static Rectangle GetRectangle(Vector2i<int> start, Vector2i<int> end)
	{
		bool startXMin = start.X < end.X;
		int minX, maxX;
		if (startXMin)
		{
			minX = start.X;
			maxX = end.X;
		}
		else
		{
			minX = end.X;
			maxX = start.X;
		}

		bool startYMin = start.Y < end.Y;
		int minY, maxY;
		if (startYMin)
		{
			minY = start.Y;
			maxY = end.Y;
		}
		else
		{
			minY = end.Y;
			maxY = start.Y;
		}

		return new(minX, minY, maxX, maxY);
	}

	private static bool LineIntersectsSquare(Vector2 lineA, Vector2 lineB, Vector2 squarePosition, float squareSize)
	{
		float squareHalfSize = squareSize / 2f;
		Vector2 squareTl = squarePosition + new Vector2(-squareHalfSize, -squareHalfSize);
		Vector2 squareTr = squarePosition + new Vector2(+squareHalfSize, -squareHalfSize);
		Vector2 squareBl = squarePosition + new Vector2(-squareHalfSize, +squareHalfSize);
		Vector2 squareBr = squarePosition + new Vector2(+squareHalfSize, +squareHalfSize);
		Vector2 lineNormal = GetLineNormal(lineA, lineB);

		// Calculate booleans for all 4 points; return true immediately when any of the booleans do not hold the same value.
		bool tlBehind = PointIsBehindPlane(lineA, lineNormal, squareTl);
		if (PointIsBehindPlane(lineA, lineNormal, squareTr) != tlBehind)
			return true;

		if (PointIsBehindPlane(lineA, lineNormal, squareBl) != tlBehind)
			return true;

		return PointIsBehindPlane(lineA, lineNormal, squareBr) != tlBehind;
	}

	private static Vector2 GetLineNormal(Vector2 lineA, Vector2 lineB)
	{
		float deltaX = lineB.X - lineA.X;
		float deltaY = lineB.Y - lineA.Y;
		return new(-deltaY, deltaX);
	}

	private static bool PointIsBehindPlane(Vector2 linePoint, Vector2 lineNormal, Vector2 point)
	{
		return Vector2.Dot(lineNormal, Vector2.Normalize(point - linePoint)) < 0;
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		Base.Game.UiRenderer.RenderTopLeft(Metric.Size, parentPosition + new Vector2i<int>(Metric.X1, Metric.Y1), Depth, Color.Black);

		for (int j = 0; j < StateManager.SpawnsetState.Spawnset.ArenaDimension; j++)
		{
			for (int k = 0; k < StateManager.SpawnsetState.Spawnset.ArenaDimension; k++)
			{
				int x = j * _tileSize;
				int y = k * _tileSize;

				Color color = TileUtils.GetColorFromHeight(StateManager.SpawnsetState.Spawnset.ArenaTiles[j, k]);
				if (color.R == 0 && color.G == 0 && color.B == 0)
					continue;

				// TODO: Optimize.
				Base.Game.UiRenderer.RenderTopLeft(new(_tileSize), parentPosition + new Vector2i<int>(Metric.X1 + x, Metric.Y1 + y), Depth + 1, color);
			}
		}
	}
}
