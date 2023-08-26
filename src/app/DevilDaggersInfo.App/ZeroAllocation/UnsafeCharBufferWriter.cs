namespace DevilDaggersInfo.App.ZeroAllocation;

[Obsolete("Use UnsafeSpan instead.")]
public struct UnsafeCharBufferWriter
{
	private readonly char[] _buffer;
	private int _position;

	public UnsafeCharBufferWriter(char[] buffer)
	{
		_buffer = buffer;
	}

	public static implicit operator ReadOnlySpan<char>(UnsafeCharBufferWriter unsafeCharBufferWriter)
	{
		return unsafeCharBufferWriter._buffer.AsSpan()[..unsafeCharBufferWriter._position];
	}

	public void Write(bool boolean)
	{
		Write(ref _position, boolean ? "True" : "False");
	}

	public void Write(ReadOnlySpan<char> text)
	{
		Write(ref _position, text);
	}

	public void Write<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		Write(ref _position, value, format, provider);
	}

	public void WriteLine(bool boolean)
	{
		Write(boolean);
		WriteLine();
	}

	public void WriteLine(ReadOnlySpan<char> text)
	{
		Write(text);
		WriteLine();
	}

	public void WriteLine<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		Write(value, format, provider);
		WriteLine();
	}

	public void WriteLine()
	{
		_buffer[_position++] = '\n';
	}

	private void Write<T>(ref int start, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		value.TryFormat(_buffer.AsSpan()[start..], out int charsWritten, format, provider);
		start += charsWritten;
	}

	private void Write(ref int start, ReadOnlySpan<char> text)
	{
		Span<char> slice = _buffer.AsSpan()[start..];
		text.CopyTo(slice);
		start += text.Length;
	}
}
