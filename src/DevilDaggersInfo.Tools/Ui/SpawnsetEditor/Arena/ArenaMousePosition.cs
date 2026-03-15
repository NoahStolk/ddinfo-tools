using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

internal readonly record struct ArenaMousePosition(Vector2 Real, Vector2D<int> Tile, bool IsValid)
{
	public static ArenaMousePosition Get(ImGuiIOPtr io, Vector2 offset, int arenaDimension)
	{
		float realX = io.MousePos.X - offset.X;
		float realY = io.MousePos.Y - offset.Y;
		Vector2 real = new(realX, realY);
		Vector2D<int> tile = new((int)Math.Floor(real.X / ArenaWindow.TileSize), (int)Math.Floor(real.Y / ArenaWindow.TileSize));
		bool isValid = tile is { X: >= 0, Y: >= 0 } && tile.X < arenaDimension && tile.Y < arenaDimension;

		return new ArenaMousePosition
		{
			Real = real,
			Tile = tile,
			IsValid = isValid,
		};
	}
}
