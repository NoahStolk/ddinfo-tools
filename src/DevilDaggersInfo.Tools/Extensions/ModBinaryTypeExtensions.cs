using DevilDaggersInfo.Core.Mod;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class ModBinaryTypeExtensions
{
	extension(ModBinaryType value)
	{
		public ReadOnlySpan<byte> AsUtf8LowerCaseSpan()
		{
			return value switch
			{
				ModBinaryType.Audio => "audio"u8,
				ModBinaryType.Dd => "dd"u8,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
			};
		}
	}
}
