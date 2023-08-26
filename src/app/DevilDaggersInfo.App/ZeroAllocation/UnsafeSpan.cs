using System.Numerics;

namespace DevilDaggersInfo.App.ZeroAllocation;

/// <summary>
/// Unsafe methods to quickly format values into a <see cref="Span{T}"/> without allocating memory.
/// These must only be used inline, as the <see cref="Span{T}"/> is only valid until the next method call.
/// </summary>
public static class UnsafeSpan
{
	public static readonly char[] Buffer = new char[512];

	public static ReadOnlySpan<char> Get(VolatileFixedInterpolatedStringHandler logMessageBuilder)
	{
		return logMessageBuilder;
	}

	public static ReadOnlySpan<char> Get<T>(T t, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		return t.TryFormat(Buffer, out int charsWritten, format, provider) ? Buffer.AsSpan()[..charsWritten] : ReadOnlySpan<char>.Empty;
	}

	public static ReadOnlySpan<char> Get(Vector2 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		// TODO: Check bounds and return in case of TryFormat returning false.
		value.X.TryFormat(Buffer, out int charsWritten, format, provider);
		Buffer[charsWritten++] = ',';
		Buffer[charsWritten++] = ' ';
		value.Y.TryFormat(Buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		charsWritten += charsWrittenY;
		return Buffer.AsSpan(0, charsWritten);
	}

	public static ReadOnlySpan<char> Get(Vector3 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		// TODO: Check bounds and return in case of TryFormat returning false.
		value.X.TryFormat(Buffer, out int charsWritten, format, provider);
		Buffer[charsWritten++] = ',';
		Buffer[charsWritten++] = ' ';
		value.Y.TryFormat(Buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		charsWritten += charsWrittenY;
		Buffer[charsWritten++] = ',';
		Buffer[charsWritten++] = ' ';
		value.Z.TryFormat(Buffer.AsSpan()[charsWritten..], out int charsWrittenZ, format, provider);
		charsWritten += charsWrittenZ;
		return Buffer.AsSpan(0, charsWritten);
	}

	public static ReadOnlySpan<char> Get(string str)
	{
		return str.AsSpan();
	}
}
