using System.Numerics;

namespace DevilDaggersInfo.App.ZeroAllocation;

public static class UnsafeSpan
{
	private static readonly char[] _buffer = new char[256];

	/// <summary>
	/// Unsafe method to quickly format a value into a <see cref="Span{T}"/> without allocating memory.
	/// This must only be used inline, as the <see cref="Span{T}"/> is only valid until the next method call.
	/// </summary>
	/// <typeparam name="T">The type of the value to format.</typeparam>
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
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		charsWritten += charsWrittenY;
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get(Vector3 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.X.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		_buffer[charsWritten + charsWrittenY++] = ',';
		value.Z.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY)..], out int charsWrittenZ, format, provider);
		charsWritten += charsWrittenY + charsWrittenZ;
		return _buffer.AsSpan(0, charsWritten);
	}
}
