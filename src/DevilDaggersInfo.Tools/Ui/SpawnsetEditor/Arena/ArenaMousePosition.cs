using DevilDaggersInfo.Tools.EditorFileState;
using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

public readonly record struct ArenaMousePosition(Vector2 Real, Vector2D<int> Tile, bool IsValid)
{
	public static ArenaMousePosition Get(ImGuiIOPtr io, Vector2 offset)
	{
		float realX = io.MousePos.X - offset.X;
		float realY = io.MousePos.Y - offset.Y;
		Vector2 real = new(realX, realY);
		Vector2D<int> tile = new((int)Math.Floor(real.X / ArenaWindow.TileSize), (int)Math.Floor(real.Y / ArenaWindow.TileSize));
		bool isValid = tile is { X: >= 0, Y: >= 0 } && tile.X < FileStates.Spawnset.Object.ArenaDimension && tile.Y < FileStates.Spawnset.Object.ArenaDimension;

		return new ArenaMousePosition
		{
			Real = real,
			Tile = tile,
			IsValid = isValid,
		};
	}
}
