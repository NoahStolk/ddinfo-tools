using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace DevilDaggersInfo.Tools;

// ReSharper disable MemberCanBeMadeStatic.Global
#pragma warning disable CA1822, RCS1163
[InterpolatedStringHandler]
internal ref struct InlineInterpolatedStringHandler
{
	private int _bytesWritten;

	public InlineInterpolatedStringHandler(int literalLength, int formattedCount)
	{
	}

	public static implicit operator ReadOnlySpan<byte>(InlineInterpolatedStringHandler handler)
	{
		return Inline.Terminate(handler._bytesWritten);
	}

	public void AppendLiteral(string s)
	{
		AppendFormatted(s.AsSpan());
	}

	public void AppendFormatted(ReadOnlySpan<char> s)
	{
		_bytesWritten += Encoding.UTF8.GetBytes(s, Inline.Buffer[_bytesWritten..]);
	}

	public void AppendFormatted(ReadOnlySpan<byte> s)
	{
		Inline.Write(ref _bytesWritten, s);
	}

	public void AppendFormatted<T>(T t, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : IUtf8SpanFormattable
	{
		Inline.Write(ref _bytesWritten, t, format, provider);
	}

	public void AppendFormatted(Vector2 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
	{
		Inline.Write(ref _bytesWritten, value.X, format, provider);
		Inline.Write(ref _bytesWritten, Inline.NumericSeparator);
		Inline.Write(ref _bytesWritten, value.Y, format, provider);
	}

	public void AppendFormatted(Vector3 value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
	{
		Inline.Write(ref _bytesWritten, value.X, format, provider);
		Inline.Write(ref _bytesWritten, Inline.NumericSeparator);
		Inline.Write(ref _bytesWritten, value.Y, format, provider);
		Inline.Write(ref _bytesWritten, Inline.NumericSeparator);
		Inline.Write(ref _bytesWritten, value.Z, format, provider);
	}
}
