using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;

public static class ArenaCanvas
{
	public static void Render()
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Vector2 origin = ImGui.GetCursorScreenPos();
		drawList.AddRectFilled(origin, origin + ArenaChild.ArenaSize, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

		// TODO: Optimize this. Maybe we can draw to a texture and then draw that texture instead.
		for (int i = 0; i < SpawnsetState.Spawnset.ArenaDimension; i++)
		{
			for (int j = 0; j < SpawnsetState.Spawnset.ArenaDimension; j++)
			{
				int x = i * ArenaChild.TileSize;
				int y = j * ArenaChild.TileSize;

				float actualHeight = SpawnsetState.Spawnset.GetActualTileHeight(i, j, ArenaChild.CurrentSecond);
				float height = SpawnsetState.Spawnset.ArenaTiles[i, j];
				Color colorCurrent = TileUtils.GetColorFromHeight(actualHeight);
				Color colorValue = TileUtils.GetColorFromHeight(height);
				Vector2 min = origin + new Vector2(x, y);

				if (Math.Abs(actualHeight - height) < 0.001f)
				{
					if (Color.Black != colorValue)
					{
						drawList.AddRectFilled(min, min + new Vector2(ArenaChild.TileSize), ImGui.GetColorU32(colorValue));
					}
				}
				else
				{
					if (Color.Black != colorCurrent)
					{
						drawList.AddRectFilled(min, min + new Vector2(ArenaChild.TileSize), ImGui.GetColorU32(colorCurrent));
					}

					if (Color.Black != colorValue)
					{
						const int offset = 2;
						const int size = ArenaChild.TileSize - offset * 2;
						drawList.AddRectFilled(min + new Vector2(offset), min + new Vector2(offset) + new Vector2(size), ImGui.GetColorU32(colorValue));
					}
				}
			}
		}

		if (SpawnsetState.Spawnset.GameMode == GameMode.Race)
		{
			int arenaMiddle = SpawnsetState.Spawnset.ArenaDimension / 2;
			float realRaceX = SpawnsetState.Spawnset.RaceDaggerPosition.X / 4f + arenaMiddle;
			float realRaceZ = SpawnsetState.Spawnset.RaceDaggerPosition.Y / 4f + arenaMiddle;

			const int halfSize = ArenaChild.TileSize / 2;

			float? actualHeight = SpawnsetState.Spawnset.GetActualRaceDaggerHeight(ArenaChild.CurrentSecond);
			Color tileColor = actualHeight.HasValue ? TileUtils.GetColorFromHeight(actualHeight.Value) : Color.Black;
			Color invertedTileColor = Color.Invert(tileColor);
			Vector3 daggerColor = Vector3.Lerp(invertedTileColor, invertedTileColor.Intensify(96), MathF.Sin((float)ImGui.GetTime()) / 2 + 0.5f);

			Vector2 center = origin + new Vector2(realRaceX * ArenaChild.TileSize + halfSize, realRaceZ * ArenaChild.TileSize + halfSize);
			drawList.AddImage((IntPtr)Root.InternalResources.IconDaggerTexture.Handle, center - new Vector2(8), center + new Vector2(8), Vector2.UnitX, Vector2.UnitY, ImGui.GetColorU32(Color.FromVector3(daggerColor)));
		}

		const int tileUnit = 4;
		Vector2 arenaCenter = origin + new Vector2((int)(SpawnsetBinary.ArenaDimensionMax / 2f * ArenaChild.TileSize));

		float shrinkStartRadius = SpawnsetState.Spawnset.ShrinkStart / tileUnit * ArenaChild.TileSize;
		if (shrinkStartRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkStartRadius, ImGui.GetColorU32(Color.Blue));

		float shrinkEndTime = SpawnsetState.Spawnset.GetShrinkEndTime();
		float shrinkRadius = shrinkEndTime == 0 ? SpawnsetState.Spawnset.ShrinkStart : Math.Max(SpawnsetState.Spawnset.ShrinkStart - ArenaChild.CurrentSecond / shrinkEndTime * (SpawnsetState.Spawnset.ShrinkStart - SpawnsetState.Spawnset.ShrinkEnd), SpawnsetState.Spawnset.ShrinkEnd);
		float shrinkCurrentRadius = shrinkRadius / tileUnit * ArenaChild.TileSize;
		if (shrinkCurrentRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkCurrentRadius, ImGui.GetColorU32(Color.Purple));

		float shrinkEndRadius = SpawnsetState.Spawnset.ShrinkEnd / tileUnit * ArenaChild.TileSize;
		if (shrinkEndRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkEndRadius, ImGui.GetColorU32(Color.Red));
	}
}
