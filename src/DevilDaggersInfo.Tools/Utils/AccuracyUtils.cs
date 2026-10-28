namespace DevilDaggersInfo.Tools.Utils;

internal static class AccuracyUtils
{
	public static float GetAccuracy(int fired, int hit)
	{
		if (fired == 0)
			return 0;

		return hit / (float)fired;
	}
}
