using System.Numerics;

namespace DevilDaggersInfo.Tools.Utils;

internal static class AccuracyUtils
{
	public static float GetAccuracy<T>(T fired, T hit)
		where T : IBinaryInteger<T>
	{
		if (fired == T.Zero)
			return 0;

		return float.CreateChecked(hit) / float.CreateChecked(fired);
	}
}
