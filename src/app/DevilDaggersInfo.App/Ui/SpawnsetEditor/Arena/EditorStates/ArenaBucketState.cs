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

	private ArenaMousePosition _cachedPosition;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (!ImGui.IsMouseClicked(ImGuiMouseButton.Left) && _cachedPosition == mousePosition)
			return;

		_cachedPosition = mousePosition;

		_targetCoords.Clear();

		int dimension = SpawnsetState.Spawnset.ArenaDimension;
		float targetHeight = SpawnsetState.Spawnset.ArenaTiles[mousePosition.Tile.X, mousePosition.Tile.Y];
		FillNeighbors(mousePosition.Tile.X, mousePosition.Tile.Y);

		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			float[,] newArena = SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
			foreach (Vector2D<int> coord in _targetCoords)
				newArena[coord.X, coord.Y] = ArenaChild.SelectedHeight;

			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, newArena) };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaBucket);
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
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		_targetCoords.Clear();
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
