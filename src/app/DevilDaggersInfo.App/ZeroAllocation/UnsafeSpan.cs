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
}
