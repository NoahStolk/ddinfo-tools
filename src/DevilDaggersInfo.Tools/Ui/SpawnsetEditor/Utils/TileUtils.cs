using DevilDaggersInfo.Tools.Extensions;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;

internal static class TileUtils
{
	public static Vector4 GetColorFromHeight(float tileHeight)
	{
		float h = tileHeight * 3 + 12;
		float s = (tileHeight + 1.5f) * 0.25f;
		float v = (tileHeight + 2) * 0.2f;
		return Vector4.CreateColorFromHsv(h, s, v);
	}
}
