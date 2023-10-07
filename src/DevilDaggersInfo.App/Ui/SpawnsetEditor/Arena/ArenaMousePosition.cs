using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;

public readonly record struct ArenaMousePosition(Vector2 Real, Vector2D<int> Tile, bool IsValid)
{
	public static ArenaMousePosition Get(ImGuiIOPtr io, Vector2 offset)
	{
		float realX = io.MousePos.X - offset.X;
		float realY = io.MousePos.Y - offset.Y;
		Vector2 real = new(realX, realY);
		Vector2D<int> tile = new((int)Math.Floor(real.X / ArenaChild.TileSize), (int)Math.Floor(real.Y / ArenaChild.TileSize));
		bool isValid = tile is { X: >= 0, Y: >= 0 } && tile.X < SpawnsetState.Spawnset.ArenaDimension && tile.Y < SpawnsetState.Spawnset.ArenaDimension;

		return new()
		{
			Real = real,
			Tile = tile,
			IsValid = isValid,
		};
	}
}
