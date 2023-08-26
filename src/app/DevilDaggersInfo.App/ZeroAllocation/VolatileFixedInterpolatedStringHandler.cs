using System.Runtime.CompilerServices;

namespace DevilDaggersInfo.App.ZeroAllocation;

// ReSharper disable MemberCanBeMadeStatic.Global
#pragma warning disable CA1822, RCS1163
[InterpolatedStringHandler]
public ref struct VolatileFixedInterpolatedStringHandler
{
	private int _charsWritten;

	public VolatileFixedInterpolatedStringHandler(int literalLength, int formattedCount)
	{
	}

	public static implicit operator ReadOnlySpan<char>(VolatileFixedInterpolatedStringHandler handler)
	{
		return UnsafeSpan.Buffer.AsSpan()[..handler._charsWritten];
	}

	public void AppendLiteral(string s)
	{
		s.CopyTo(0, UnsafeSpan.Buffer, _charsWritten, s.Length);
		_charsWritten += s.Length;
	}

	public void AppendFormatted<T>(T t, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		t.TryFormat(UnsafeSpan.Buffer.AsSpan()[_charsWritten..], out int charsWritten, format, provider);
		_charsWritten += charsWritten;
	}
}
