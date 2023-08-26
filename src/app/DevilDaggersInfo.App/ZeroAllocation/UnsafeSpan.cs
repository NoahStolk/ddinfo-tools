using System.Numerics;

namespace DevilDaggersInfo.App.ZeroAllocation;

/// <summary>
/// Unsafe methods to quickly format values into a <see cref="Span{T}"/> without allocating memory.
/// These must only be used inline, as the <see cref="Span{T}"/> is only valid until the next method call.
/// </summary>
public static class UnsafeSpan
{
	private static readonly char[] _buffer = new char[1024];

	public static Span<char> Get<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
		where T : ISpanFormattable
	{
		Array.Clear(_buffer);

		value.TryFormat(_buffer, out int charsWritten, format, provider);
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get(Vector2 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.X.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		_buffer[charsWritten++] = ' ';
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		charsWritten += charsWrittenY;
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get(Vector3 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.X.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		_buffer[charsWritten++] = ' ';
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		charsWritten += charsWrittenY;
		_buffer[charsWritten++] = ',';
		_buffer[charsWritten++] = ' ';
		value.Z.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenZ, format, provider);
		charsWritten += charsWrittenZ;
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get<T>(ReadOnlySpan<char> value1, ReadOnlySpan<char> value2, T value3, ReadOnlySpan<char> value4, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
		where T : ISpanFormattable
	{
		Array.Clear(_buffer);

		int charsWritten = 0;

		value1.TryCopyTo(_buffer);
		charsWritten += value1.Length;

		value2.TryCopyTo(_buffer.AsSpan()[charsWritten..]);
		charsWritten += value2.Length;

		value3.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenValue3, format, provider);
		charsWritten += charsWrittenValue3;

		value4.TryCopyTo(_buffer.AsSpan()[charsWritten..]);
		charsWritten += value4.Length;

		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get<T>(T value1, ReadOnlySpan<char> value2, ReadOnlySpan<char> value3, ReadOnlySpan<char> value4, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
		where T : ISpanFormattable
	{
		Array.Clear(_buffer);

		int charsWritten = 0;

		value1.TryFormat(_buffer, out int charsWrittenValue1, format, provider);
		charsWritten += charsWrittenValue1;

		value2.TryCopyTo(_buffer.AsSpan()[charsWritten..]);
		charsWritten += value2.Length;

		value3.TryCopyTo(_buffer.AsSpan()[charsWritten..]);
		charsWritten += value3.Length;

		value4.TryCopyTo(_buffer.AsSpan()[charsWritten..]);
		charsWritten += value4.Length;

		return _buffer.AsSpan(0, charsWritten);
	}
}
