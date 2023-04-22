using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Types.Core.Spawnsets;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;

public class Arena : AbstractComponent
{
	public const int TileSize = 6;

	private readonly ArenaPencilState _pencilState;
	private readonly ArenaLineState _lineState;
	private readonly ArenaRectangleState _rectangleState;
	private readonly ArenaEllipseState _ellipseState;
	private readonly ArenaBucketState _bucketState;
	private readonly ArenaDaggerState _daggerState;

	private float _currentSecond;
	private float _shrinkRadius;

	public Arena(IBounds bounds)
		: base(bounds)
	{
		_pencilState = new();
		_lineState = new();
		_rectangleState = new();
		_ellipseState = new();
		_bucketState = new();
		_daggerState = new();
	}

	public static Vector2i<int> HalfTile { get; } = new(TileSize / 2);
	public static Vector2 HalfTileAsVector2 { get; } = new(TileSize / 2f);

	private IArenaState GetActiveState() => StateManager.ArenaEditorState.ArenaTool switch
	{
		ArenaTool.Pencil => _pencilState,
		ArenaTool.Line => _lineState,
		ArenaTool.Rectangle => _rectangleState,
		ArenaTool.Ellipse => _ellipseState,
		ArenaTool.Bucket => _bucketState,
		ArenaTool.Dagger => _daggerState,
		_ => throw new UnreachableException(),
	};

	private ArenaMousePosition GetArenaMousePosition(Vector2i<int> scrollOffset)
	{
		int realX = (int)MouseUiContext.MousePosition.X - Bounds.X1 - scrollOffset.X;
		int realY = (int)MouseUiContext.MousePosition.Y - Bounds.Y1 - scrollOffset.Y;
		return new()
		{
			Real = new(realX, realY),
			Tile = new(realX / TileSize, realY / TileSize),
		};
	}

	private static void UpdateArena(int x, int y, float height, SpawnsetEditType spawnsetEditType)
	{
		float[,] newArena = StateManager.SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		newArena[x, y] = height;
		UpdateArena(newArena, spawnsetEditType);
	}

	public static void UpdateArena(float[,] newArena, SpawnsetEditType spawnsetEditType)
	{
		StateManager.Dispatch(new UpdateArena(newArena, spawnsetEditType));
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		ArenaMousePosition mousePosition = GetArenaMousePosition(scrollOffset);
		bool isOutOfRange = mousePosition.Tile.X < 0 || mousePosition.Tile.Y < 0 || mousePosition.Tile.X >= StateManager.SpawnsetState.Spawnset.ArenaDimension || mousePosition.Tile.Y >= StateManager.SpawnsetState.Spawnset.ArenaDimension;
		IArenaState activeState = GetActiveState();
		if (isOutOfRange)
		{
			activeState.HandleOutOfRange(mousePosition);
		}
		else
		{
			Root.Game.TooltipContext = new()
			{
				Text = $"{StateManager.SpawnsetState.Spawnset.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y]}\n{{{mousePosition.Tile.X}, {mousePosition.Tile.Y}}}",
				ForegroundColor = Color.HalfTransparentWhite,
				BackgroundColor = Color.HalfTransparentBlack,
			};
			int scroll = Input.GetScroll();
			if (scroll != 0)
			{
				// TODO: Selection.
				UpdateArena(mousePosition.Tile.X, mousePosition.Tile.Y, StateManager.SpawnsetState.Spawnset.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y] - scroll, SpawnsetEditType.ArenaTileHeight);
			}

			activeState.Handle(mousePosition);
		}
	}

	public void SetShrinkCurrent(float currentSecond)
	{
		_currentSecond = currentSecond;

		SpawnsetBinary spawnset = StateManager.SpawnsetState.Spawnset;
		float shrinkEndTime = spawnset.GetShrinkEndTime();
		_shrinkRadius = shrinkEndTime == 0 ? spawnset.ShrinkStart : Math.Max(spawnset.ShrinkStart - _currentSecond / shrinkEndTime * (spawnset.ShrinkStart - spawnset.ShrinkEnd), spawnset.ShrinkEnd);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> origin = scrollOffset + new Vector2i<int>(Bounds.X1, Bounds.Y1);
		Vector2i<int> center = origin + new Vector2i<int>((int)(SpawnsetBinary.ArenaDimensionMax / 2f * TileSize));
		Vector2i<int> halfTileSize = new Vector2i<int>(TileSize, TileSize) / 2;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, center, Depth, Color.Black);

		for (int i = 0; i < StateManager.SpawnsetState.Spawnset.ArenaDimension; i++)
		{
			for (int j = 0; j < StateManager.SpawnsetState.Spawnset.ArenaDimension; j++)
			{
				int x = i * TileSize;
				int y = j * TileSize;

				float actualHeight = StateManager.SpawnsetState.Spawnset.GetActualTileHeight(i, j, _currentSecond);
				float height = StateManager.SpawnsetState.Spawnset.ArenaTiles[i, j];
				Color colorCurrent = TileUtils.GetColorFromHeight(actualHeight);
				Color colorValue = TileUtils.GetColorFromHeight(height);
				if (Math.Abs(actualHeight - height) < 0.001f)
				{
					if (Color.Black != colorValue)
						Root.Game.RectangleRenderer.Schedule(new(TileSize), origin + new Vector2i<int>(x, y) + halfTileSize, Depth + 1, colorValue);
				}
				else
				{
					if (Color.Black != colorCurrent)
						Root.Game.RectangleRenderer.Schedule(new(TileSize), origin + new Vector2i<int>(x, y) + halfTileSize, Depth + 1, colorCurrent);

					if (Color.Black != colorValue)
					{
						const int size = 2;
						Root.Game.RectangleRenderer.Schedule(new(size), origin + new Vector2i<int>(x, y) + halfTileSize, Depth + 2, colorValue);
					}
				}
			}
		}

		if (StateManager.SpawnsetState.Spawnset.GameMode == GameMode.Race)
		{
			int arenaMiddle = StateManager.SpawnsetState.Spawnset.ArenaDimension / 2;
			float realRaceX = StateManager.SpawnsetState.Spawnset.RaceDaggerPosition.X / 4f + arenaMiddle;
			float realRaceZ = StateManager.SpawnsetState.Spawnset.RaceDaggerPosition.Y / 4f + arenaMiddle;

			const int halfSize = TileSize / 2;

			(int raceX, _, int raceZ) = StateManager.SpawnsetState.Spawnset.GetRaceDaggerTilePosition();
			float actualHeight = StateManager.SpawnsetState.Spawnset.GetActualTileHeight(raceX, raceZ, _currentSecond);
			Color tileColor = TileUtils.GetColorFromHeight(actualHeight);
			Color invertedTileColor = Color.Invert(tileColor);
			Vector3 daggerColor = Vector3.Lerp(invertedTileColor, invertedTileColor.Intensify(96), MathF.Sin(Root.Game.Tt) / 2 + 0.5f);
			Root.Game.SpriteRenderer.Schedule(new(-8, -8), origin.ToVector2() + new Vector2(realRaceX * TileSize + halfSize, realRaceZ * TileSize + halfSize), Depth + 3, ContentManager.Content.IconDaggerTexture, Color.FromVector3(daggerColor));
		}

		ScissorScheduler.PushScissor(Scissor.Create(Bounds, scrollOffset, ViewportState.Offset, ViewportState.Scale));

		const int tileUnit = 4;
		float shrinkStartRadius = StateManager.SpawnsetState.Spawnset.ShrinkStart / tileUnit * TileSize;
		float shrinkCurrentRadius = _shrinkRadius / tileUnit * TileSize;
		float shrinkEndRadius = StateManager.SpawnsetState.Spawnset.ShrinkEnd / tileUnit * TileSize;

		if (shrinkStartRadius > 0)
			Root.Game.EllipseRenderer.Schedule(center, shrinkStartRadius, Depth + 5, GlobalColors.ShrinkStart);
		if (shrinkCurrentRadius > 0)
			Root.Game.EllipseRenderer.Schedule(center, shrinkCurrentRadius, Depth + 4, GlobalColors.ShrinkCurrent);
		if (shrinkEndRadius > 0)
			Root.Game.EllipseRenderer.Schedule(center, shrinkEndRadius, Depth + 5, GlobalColors.ShrinkEnd);

		IArenaState activeState = GetActiveState();
		activeState.Render(GetArenaMousePosition(scrollOffset), origin, Depth + 3);

		ScissorScheduler.PopScissor();
	}
}
