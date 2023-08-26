namespace DevilDaggersInfo.App.ZeroAllocation;

[Obsolete("Use UnsafeSpan instead.")]
public class IdBuffer
{
	private readonly char[] _buffer;

	public IdBuffer(int size)
	{
		_buffer = new char[size];
	}

	public static implicit operator ReadOnlySpan<char>(IdBuffer idBuffer) => idBuffer._buffer;

	public void Overwrite(ReadOnlySpan<char> buffer1, int index)
	{
		Overwrite(buffer1, UnsafeSpan.Get(index));
	}

	public void Overwrite(ReadOnlySpan<char> buffer1, ReadOnlySpan<char> buffer2)
	{
		Span<char> span = _buffer.AsSpan();
		span.Clear();
		buffer1.CopyTo(_buffer);
		buffer2.CopyTo(span[buffer1.Length..]);
	}
}
