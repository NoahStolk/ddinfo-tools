using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaBucketState : IArenaState
{
	private readonly HashSet<Vector2D<int>> _targetCoords = new();
	private Vector2D<int> _cachedPosition;
	private bool _isFilling;

	private void FillNeighbors(int x, int y)
	{
		int dimension = SpawnsetState.Spawnset.ArenaDimension;
		if (x < 0 || y < 0 || x >= dimension || y >= dimension)
			return;

		float targetHeight = SpawnsetState.Spawnset.ArenaTiles[x, y];

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

			float tileHeight = SpawnsetState.Spawnset.ArenaTiles[newX, newY];

			float clampedTargetHeight = targetHeight;
			if (targetHeight < BucketChild.VoidHeight)
				clampedTargetHeight = BucketChild.VoidHeight;
			if (tileHeight < BucketChild.VoidHeight)
				tileHeight = BucketChild.VoidHeight;

			if (MathF.Abs(tileHeight - clampedTargetHeight) < BucketChild.Tolerance)
				FillNeighbors(newX, newY);
		}
	}

	private void SaveCurrentFill()
	{
		if (_isFilling)
			return;

		_isFilling = true;
		float[,] newArena = SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		foreach (Vector2D<int> coord in _targetCoords)
			newArena[coord.X, coord.Y] = ArenaChild.SelectedHeight;

		SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, newArena) };
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaBucket);
	}

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		FillNeighbors(mousePosition.Tile.X, mousePosition.Tile.Y);
		SaveCurrentFill();
	}

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && _isFilling)
			_isFilling = false;

		if (!ImGui.IsMouseDown(ImGuiMouseButton.Left) && _cachedPosition == mousePosition.Tile)
			return;

		_targetCoords.Clear();
		_cachedPosition = mousePosition.Tile;

		FillNeighbors(mousePosition.Tile.X, mousePosition.Tile.Y);
		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
			SaveCurrentFill();
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		_targetCoords.Clear();
		_isFilling = false;
	}

	public void Render(ArenaMousePosition mousePosition)
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Vector2 origin = ImGui.GetCursorScreenPos();
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				if (_targetCoords.Contains(new(i, j)))
				{
					Vector2 topLeft = origin + new Vector2(i, j) * ArenaChild.TileSize;
					drawList.AddRectFilled(topLeft, topLeft + new Vector2(ArenaChild.TileSize), ImGui.GetColorU32(Color.HalfTransparentWhite));
				}
			}
		}
	}
}
