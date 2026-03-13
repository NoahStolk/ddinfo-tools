namespace DevilDaggersInfo.Tools.Engine.Maths;

public static class MathUtils
{
	public static float Lerp(float value1, float value2, float amount)
	{
		return value1 + (value2 - value1) * amount;
	}

	public static T Max<T>(ReadOnlySpan<T> values)
		where T : struct, IComparable<T>
	{
		if (values.IsEmpty)
			return default;

		T max = values[0];
		for (int i = 1; i < values.Length; i++)
		{
			if (values[i].CompareTo(max) > 0)
				max = values[i];
		}

		return max;
	}
}
