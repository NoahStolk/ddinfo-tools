using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class DaggerTypeExtensions
{
	extension(DaggerType)
	{
		public static ReadOnlySpan<byte> DaggerTypeNullTerminatedDisplayNames => "Lvl1\0Lvl2\0Lvl3\0Lvl3 Homing\0Lvl4\0Lvl4 Homing\0Lvl4 Splash\0"u8;
	}
}
