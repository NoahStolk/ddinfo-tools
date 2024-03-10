using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

public static class ArenaCanvas
{
	public static void Render()
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Vector2 origin = ImGui.GetCursorScreenPos();
		drawList.AddRectFilled(origin, origin + ArenaWindow.ArenaSize, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

		// TODO: Optimize this. Maybe we can draw to a texture and then draw that texture instead.
		for (int i = 0; i < FileStates.Spawnset.Object.ArenaDimension; i++)
		{
			for (int j = 0; j < FileStates.Spawnset.Object.ArenaDimension; j++)
			{
				int x = i * ArenaWindow.TileSize;
				int y = j * ArenaWindow.TileSize;

				float actualHeight = FileStates.Spawnset.Object.GetActualTileHeight(i, j, ArenaWindow.CurrentSecond);
				float height = FileStates.Spawnset.Object.ArenaTiles[i, j];
				Color colorCurrent = TileUtils.GetColorFromHeight(actualHeight);
				Color colorValue = TileUtils.GetColorFromHeight(height);
				Vector2 min = origin + new Vector2(x, y);

				if (Math.Abs(actualHeight - height) < 0.001f)
				{
					if (Color.Black != colorValue)
					{
						drawList.AddRectFilled(min, min + new Vector2(ArenaWindow.TileSize), ImGui.GetColorU32(colorValue));
					}
				}
				else
				{
					if (Color.Black != colorCurrent)
					{
						drawList.AddRectFilled(min, min + new Vector2(ArenaWindow.TileSize), ImGui.GetColorU32(colorCurrent));
					}

					if (Color.Black != colorValue)
					{
						const int offset = 2;
						const int size = ArenaWindow.TileSize - offset * 2;
						drawList.AddRectFilled(min + new Vector2(offset), min + new Vector2(offset) + new Vector2(size), ImGui.GetColorU32(colorValue));
					}
				}
			}
		}

		if (FileStates.Spawnset.Object.GameMode == GameMode.Race)
		{
			int arenaMiddle = FileStates.Spawnset.Object.ArenaDimension / 2;
			float realRaceX = FileStates.Spawnset.Object.RaceDaggerPosition.X / 4f + arenaMiddle;
			float realRaceZ = FileStates.Spawnset.Object.RaceDaggerPosition.Y / 4f + arenaMiddle;

			const int halfSize = ArenaWindow.TileSize / 2;

			float? actualHeight = FileStates.Spawnset.Object.GetActualRaceDaggerHeight(ArenaWindow.CurrentSecond);
			Color tileColor = actualHeight.HasValue ? TileUtils.GetColorFromHeight(actualHeight.Value) : Color.Black;
			Color invertedTileColor = Color.Invert(tileColor);
			Vector3 daggerColor = Vector3.Lerp(invertedTileColor, invertedTileColor.Intensify(96), MathF.Sin((float)ImGui.GetTime()) / 2 + 0.5f);

			Vector2 center = origin + new Vector2(realRaceX * ArenaWindow.TileSize + halfSize, realRaceZ * ArenaWindow.TileSize + halfSize);
			drawList.AddImage(Root.GameResources.IconMaskDaggerTexture.Id, center - new Vector2(8), center + new Vector2(8), Color.FromVector3(daggerColor));
		}

		const int tileUnit = 4;
		Vector2 arenaCenter = origin + new Vector2((int)(SpawnsetBinary.ArenaDimensionMax / 2f * ArenaWindow.TileSize));

		float shrinkStartRadius = FileStates.Spawnset.Object.ShrinkStart / tileUnit * ArenaWindow.TileSize;
		if (shrinkStartRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkStartRadius, ImGui.GetColorU32(Color.Blue));

		float shrinkEndTime = FileStates.Spawnset.Object.GetShrinkEndTime();
		float shrinkRadius = shrinkEndTime == 0 ? FileStates.Spawnset.Object.ShrinkStart : Math.Max(FileStates.Spawnset.Object.ShrinkStart - ArenaWindow.CurrentSecond / shrinkEndTime * (FileStates.Spawnset.Object.ShrinkStart - FileStates.Spawnset.Object.ShrinkEnd), FileStates.Spawnset.Object.ShrinkEnd);
		float shrinkCurrentRadius = shrinkRadius / tileUnit * ArenaWindow.TileSize;
		if (shrinkCurrentRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkCurrentRadius, ImGui.GetColorU32(Color.Purple));

		float shrinkEndRadius = FileStates.Spawnset.Object.ShrinkEnd / tileUnit * ArenaWindow.TileSize;
		if (shrinkEndRadius is > 0 and < 300)
			drawList.AddCircle(arenaCenter, shrinkEndRadius, ImGui.GetColorU32(Color.Red));
	}
}
