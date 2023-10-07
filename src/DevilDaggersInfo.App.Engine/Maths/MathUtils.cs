namespace DevilDaggersInfo.App.Engine.Maths;

public static class MathUtils
{
	private const float _toRad = MathF.PI / 180;
	private const float _toDeg = 180 / MathF.PI;

	public static float ToDegrees(float radians)
		=> radians * _toDeg;

	public static float ToRadians(float degrees)
		=> degrees * _toRad;

	public static float Lerp(float value1, float value2, float amount)
		=> value1 + (value2 - value1) * amount;

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
