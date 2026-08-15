using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace DevilDaggersInfo.Tools;

/// <summary>
/// <para>
/// Unsafe methods to quickly format values into a fixed UTF-8 buffer, without allocating heap memory.
/// These must only be used inline, as the buffer is only valid until the next method call.
/// </para>
/// <para>
/// The returned spans are null-terminated, because ImGui reads the underlying memory directly. The terminator sits
/// just past the end of the returned span, so it is never included in the length.
/// </para>
/// <para>
/// The buffer has a fixed size of 2048 bytes, so the total length of the formatted string cannot exceed this limit.
/// </para>
/// </summary>
internal static class Inline
{
	private static readonly byte[] _buffer = new byte[2048];

	internal static ReadOnlySpan<byte> NumericSeparator => ", "u8;

	public static Span<byte> Buffer => _buffer;

	public static ReadOnlySpan<byte> Utf8(InlineInterpolatedStringHandler interpolatedStringHandler)
	{
		return interpolatedStringHandler;
	}

	public static ReadOnlySpan<byte> Utf8<T>(T t, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : IUtf8SpanFormattable
	{
		int bytesWritten = 0;
		Write(ref bytesWritten, t, format, provider);
		return Terminate(bytesWritten);
	}

	public static ReadOnlySpan<byte> Utf8(Vector2 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
	{
		int bytesWritten = 0;
		Write(ref bytesWritten, value.X, format, provider);
		Write(ref bytesWritten, NumericSeparator);
		Write(ref bytesWritten, value.Y, format, provider);
		return Terminate(bytesWritten);
	}

	public static ReadOnlySpan<byte> Utf8(Vector3 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
	{
		int bytesWritten = 0;
		Write(ref bytesWritten, value.X, format, provider);
		Write(ref bytesWritten, NumericSeparator);
		Write(ref bytesWritten, value.Y, format, provider);
		Write(ref bytesWritten, NumericSeparator);
		Write(ref bytesWritten, value.Z, format, provider);
		return Terminate(bytesWritten);
	}

	public static ReadOnlySpan<byte> Utf8(scoped ReadOnlySpan<char> str)
	{
		int bytesWritten = Encoding.UTF8.GetBytes(str, Buffer);
		return Terminate(bytesWritten);
	}

	/// <summary>
	/// Formats a value that only implements <see cref="ISpanFormattable" />, by formatting to UTF-16 first and then
	/// transcoding. Enums and the numeric types from DevilDaggersInfo.Core take this path; anything implementing
	/// <see cref="IUtf8SpanFormattable" /> should use <see cref="Utf8{T}" /> instead, which formats directly to UTF-8.
	/// </summary>
	public static ReadOnlySpan<byte> Utf8Formattable<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		Span<char> chars = stackalloc char[512];
		if (!value.TryFormat(chars, out int charsWritten, format, provider))
			throw new InvalidOperationException("The formatted string is too long.");

		return Utf8(chars[..charsWritten]);
	}

	/// <summary>
	/// Null-terminates the buffer just past the written bytes, and returns the written bytes without the terminator.
	/// </summary>
	internal static ReadOnlySpan<byte> Terminate(int bytesWritten)
	{
		if (bytesWritten >= Buffer.Length)
			throw new InvalidOperationException("The formatted string is too long.");

		Buffer[bytesWritten] = 0x00;
		return Buffer[..bytesWritten];
	}

	internal static void Write(ref int bytesWritten, ReadOnlySpan<byte> value)
	{
		// Appending a slice of the buffer back into itself would read from memory that is about to be overwritten,
		// which silently produces wrong text rather than crashing. Callers must pass a u8 literal or an unrelated span.
		Debug.Assert(!value.Overlaps(Buffer), "Cannot append a slice of the Inline buffer into itself.");

		if (!value.TryCopyTo(Buffer[bytesWritten..]))
			throw new InvalidOperationException("The formatted string is too long.");

		bytesWritten += value.Length;
	}

	internal static void Write<T>(ref int bytesWritten, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : IUtf8SpanFormattable
	{
		if (!value.TryFormat(Buffer[bytesWritten..], out int bytesWrittenValue, format, provider))
			throw new InvalidOperationException("The formatted string is too long.");

		bytesWritten += bytesWrittenValue;
	}
}
