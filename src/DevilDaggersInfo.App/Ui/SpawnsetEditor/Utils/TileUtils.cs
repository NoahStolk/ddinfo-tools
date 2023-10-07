using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;

public static class TileUtils
{
	public static Color GetColorFromHeight(float tileHeight)
	{
		float h = tileHeight * 3 + 12;
		float s = (tileHeight + 1.5f) * 0.25f;
		float v = (tileHeight + 2) * 0.2f;
		return Color.FromHsv(h, s, v);
	}
}
