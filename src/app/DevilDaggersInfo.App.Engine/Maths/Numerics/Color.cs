namespace DevilDaggersInfo.App.Engine.Maths.Numerics;

public readonly record struct Color(byte R, byte G, byte B, byte A)
{
	public static Color Invisible { get; } = new(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);

	public static Color White { get; } = new(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
	public static Color Black { get; } = new(byte.MinValue, byte.MinValue, byte.MinValue, byte.MaxValue);

	public static Color HalfTransparentWhite { get; } = new(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue / 2);
	public static Color HalfTransparentBlack { get; } = new(byte.MinValue, byte.MinValue, byte.MinValue, byte.MaxValue / 2);

	public static Color Red { get; } = new(byte.MaxValue, byte.MinValue, byte.MinValue, byte.MaxValue);
	public static Color Green { get; } = new(byte.MinValue, byte.MaxValue, byte.MinValue, byte.MaxValue);
	public static Color Blue { get; } = new(byte.MinValue, byte.MinValue, byte.MaxValue, byte.MaxValue);

	public static Color Yellow { get; } = new(byte.MaxValue, byte.MaxValue, byte.MinValue, byte.MaxValue);
	public static Color Aqua { get; } = new(byte.MinValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
	public static Color Purple { get; } = new(byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MaxValue);

	public static Color Orange { get; } = new(byte.MaxValue, byte.MaxValue / 2, byte.MinValue, byte.MaxValue);

	public static implicit operator Vector3(Color color)
		=> new(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue);

	public static implicit operator Vector4(Color color)
		=> new(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue, color.A / (float)byte.MaxValue);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color operator +(Color left, Color right)
		=> new((byte)(left.R + right.R), (byte)(left.G + right.G), (byte)(left.B + right.B), (byte)(left.A + right.A));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color operator -(Color left, Color right)
		=> new((byte)(left.R - right.R), (byte)(left.G - right.G), (byte)(left.B - right.B), (byte)(left.A - right.A));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color operator *(Color left, Color right)
		=> new((byte)(left.R * right.R), (byte)(left.G * right.G), (byte)(left.B * right.B), (byte)(left.A * right.A));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color operator /(Color left, Color right)
		=> new((byte)(left.R / right.R), (byte)(left.G / right.G), (byte)(left.B / right.B), (byte)(left.A / right.A));

	public Color ReadableColorForBrightness()
	{
		return GetPerceivedBrightness() < 140 ? White : Black;
	}

	public int GetPerceivedBrightness()
	{
		return (int)Math.Sqrt(R * R * 0.299 + G * G * 0.587 + B * B * 0.114);
	}

	public Color Intensify(byte component)
	{
		byte r = (byte)Math.Min(byte.MaxValue, R + component);
		byte g = (byte)Math.Min(byte.MaxValue, G + component);
		byte b = (byte)Math.Min(byte.MaxValue, B + component);
		byte a = (byte)Math.Min(byte.MaxValue, A + component);
		return new(r, g, b, a);
	}

	public int GetHue()
	{
		byte min = Math.Min(Math.Min(R, G), B);
		byte max = Math.Max(Math.Max(R, G), B);

		if (min == max)
			return 0;

		float hue;
		if (max == R)
			hue = (G - B) / (float)(max - min);
		else if (max == G)
			hue = 2f + (B - R) / (float)(max - min);
		else
			hue = 4f + (R - G) / (float)(max - min);

		hue *= 60;
		if (hue < 0)
			hue += 360;

		return (int)Math.Round(hue);
	}

	public static Color Lerp(Color value1, Color value2, float amount)
	{
		float r = MathUtils.Lerp(value1.R, value2.R, amount);
		float g = MathUtils.Lerp(value1.G, value2.G, amount);
		float b = MathUtils.Lerp(value1.B, value2.B, amount);
		float a = MathUtils.Lerp(value1.A, value2.A, amount);
		return new((byte)r, (byte)g, (byte)b, (byte)a);
	}

	public static Color Invert(Color color)
		=> new((byte)(byte.MaxValue - color.R), (byte)(byte.MaxValue - color.G), (byte)(byte.MaxValue - color.B), color.A);

	public static Color Gray(float value)
	{
		if (value is < 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(value));

		byte component = (byte)(value * byte.MaxValue);
		return new(component, component, component, byte.MaxValue);
	}

	public static Color FromHsv(float hue, float saturation, float value)
	{
		saturation = Math.Clamp(saturation, 0, 1);
		value = Math.Clamp(value, 0, 1);

		int hi = (int)MathF.Floor(hue / 60) % 6;
		float f = hue / 60 - MathF.Floor(hue / 60);

		value *= byte.MaxValue;
		byte v = (byte)value;
		byte p = (byte)(value * (1 - saturation));
		byte q = (byte)(value * (1 - f * saturation));
		byte t = (byte)(value * (1 - (1 - f) * saturation));

		return hi switch
		{
			0 => new(v, t, p, byte.MaxValue),
			1 => new(q, v, p, byte.MaxValue),
			2 => new(p, v, t, byte.MaxValue),
			3 => new(p, q, v, byte.MaxValue),
			4 => new(t, p, v, byte.MaxValue),
			_ => new(v, p, q, byte.MaxValue),
		};
	}

	public static Color FromVector3(Vector3 vector)
	{
		return new((byte)(vector.X * byte.MaxValue), (byte)(vector.Y * byte.MaxValue), (byte)(vector.Z * byte.MaxValue), byte.MaxValue);
	}

	public static Color FromVector4(Vector4 vector)
	{
		return new((byte)(vector.X * byte.MaxValue), (byte)(vector.Y * byte.MaxValue), (byte)(vector.Z * byte.MaxValue), (byte)(vector.W * byte.MaxValue));
	}
}
