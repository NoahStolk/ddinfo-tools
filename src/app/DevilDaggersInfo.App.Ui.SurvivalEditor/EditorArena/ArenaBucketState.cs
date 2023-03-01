using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;
using Warp.NET;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaBucketState : IArenaState
{
	private readonly HashSet<Vector2i<int>> _targetCoords = new();

	private ArenaMousePosition _cachedPosition;

	public void Handle(ArenaMousePosition mousePosition)
	{
		bool leftClick = Input.IsButtonPressed(MouseButton.Left);
		if (!leftClick && _cachedPosition == mousePosition)
			return;

		_cachedPosition = mousePosition;

		_targetCoords.Clear();

		int dimension = StateManager.SpawnsetState.Spawnset.ArenaDimension;
		float targetHeight = StateManager.SpawnsetState.Spawnset.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y];
		FillNeighbors(mousePosition.Tile.X, mousePosition.Tile.Y);

		if (leftClick)
		{
			float[,] tiles = StateManager.SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
			foreach (Vector2i<int> coord in _targetCoords)
				tiles[coord.X, coord.Y] = StateManager.ArenaEditorState.SelectedHeight;

			Arena.UpdateArena(tiles, SpawnsetEditType.ArenaBucket);
		}

		void FillNeighbors(int x, int y)
		{
			_targetCoords.Add(new(x, y));

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
				if (_targetCoords.Contains(new(newX, newY)))
					return;

				float tileHeight = StateManager.SpawnsetState.Spawnset.ArenaTiles[newX, newY];

				float clampedTargetHeight = targetHeight;
				if (targetHeight < StateManager.ArenaBucketState.VoidHeight)
					clampedTargetHeight = StateManager.ArenaBucketState.VoidHeight;
				if (tileHeight < StateManager.ArenaBucketState.VoidHeight)
					tileHeight = StateManager.ArenaBucketState.VoidHeight;

				if (MathF.Abs(tileHeight - clampedTargetHeight) < StateManager.ArenaBucketState.Tolerance)
					FillNeighbors(newX, newY);
			}
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		_targetCoords.Clear();
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				if (_targetCoords.Contains(new(i, j)))
					Root.Game.RectangleRenderer.Schedule(new(Arena.TileSize), origin + new Vector2i<int>(i, j) * Arena.TileSize + Arena.HalfTile, depth, Color.HalfTransparentWhite);
			}
		}
	}
}
