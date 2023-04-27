using DevilDaggersInfo.App.Ui.Scene.GameObjects;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.Scene;

public interface IArenaScene
{
	Camera Camera { get; }
	List<Tile> Tiles { get; }
	List<LightObject> Lights { get; }
	RaceDagger? RaceDagger { get; set; }

	public void AddArena(SpawnsetBinary spawnset)
	{
		Lights.Add(new(64, default, new(1, 0.5f, 0)));

		int halfSize = spawnset.ArenaDimension / 2;
		for (int i = 0; i < spawnset.ArenaDimension; i++)
		{
			for (int j = 0; j < spawnset.ArenaDimension; j++)
			{
				float y = spawnset.ArenaTiles[i, j];
				if (y < -2)
					continue;

				float x = (i - halfSize) * 4;
				float z = (j - halfSize) * 4;
				Tiles.Add(new(x, z, i, j, Camera));
			}
		}

		RaceDagger = GetRaceDagger();

		RaceDagger? GetRaceDagger()
		{
			if (spawnset.GameMode != GameMode.Race)
				return null;

			return new(spawnset);
		}
	}
}
