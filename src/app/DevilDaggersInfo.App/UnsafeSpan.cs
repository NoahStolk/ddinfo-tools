using DevilDaggersInfo.App.Engine.Maths.Numerics;
using System.Numerics;

namespace DevilDaggersInfo.App;

public static class UnsafeSpan
{
	private static readonly char[] _buffer = new char[256];

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

	public static Span<char> Get(Vector4 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.X.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		_buffer[charsWritten + charsWrittenY++] = ',';
		value.Z.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY)..], out int charsWrittenZ, format, provider);
		_buffer[charsWritten + charsWrittenY + charsWrittenZ++] = ',';
		value.W.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY + charsWrittenZ)..], out int charsWrittenW, format, provider);
		charsWritten += charsWrittenY + charsWrittenZ + charsWrittenW;
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get(Quaternion value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.X.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		value.Y.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		_buffer[charsWritten + charsWrittenY++] = ',';
		value.Z.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY)..], out int charsWrittenZ, format, provider);
		_buffer[charsWritten + charsWrittenY + charsWrittenZ++] = ',';
		value.W.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY + charsWrittenZ)..], out int charsWrittenW, format, provider);
		charsWritten += charsWrittenY + charsWrittenZ + charsWrittenW;
		return _buffer.AsSpan(0, charsWritten);
	}

	public static Span<char> Get(Color value, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
	{
		Array.Clear(_buffer);

		value.R.TryFormat(_buffer, out int charsWritten, format, provider);
		_buffer[charsWritten++] = ',';
		value.G.TryFormat(_buffer.AsSpan()[charsWritten..], out int charsWrittenY, format, provider);
		_buffer[charsWritten + charsWrittenY++] = ',';
		value.B.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY)..], out int charsWrittenZ, format, provider);
		_buffer[charsWritten + charsWrittenY + charsWrittenZ++] = ',';
		value.A.TryFormat(_buffer.AsSpan()[(charsWritten + charsWrittenY + charsWrittenZ)..], out int charsWrittenW, format, provider);
		charsWritten += charsWrittenY + charsWrittenZ + charsWrittenW;
		return _buffer.AsSpan(0, charsWritten);
	}
}
