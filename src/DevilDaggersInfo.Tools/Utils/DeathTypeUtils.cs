namespace DevilDaggersInfo.Tools.Utils;

internal static class DeathTypeUtils
{
	public static string InterpretDeathType(byte deathType)
	{
		return deathType switch
		{
			0 => "FALLEN",
			1 => "SWARMED",
			2 => "IMPALED",
			3 => "GORED",
			4 => "INFESTED",
			5 => "OPENED",
			6 => "PURGED",
			7 => "DESECRATED",
			8 => "SACRIFICED",
			9 => "EVISCERATED",
			10 => "ANNIHILATED",
			11 => "INTOXICATED",
			12 => "ENVENOMATED",
			13 => "INCARNATED",
			14 => "DISCARNATED",
			15 => "ENTANGLED",
			16 => "HAUNTED",
			_ => $"Invalid ({deathType})",
		};
	}

	public static ReadOnlySpan<byte> GetDeathTypeSpan(int deathType)
	{
		return deathType switch
		{
			0 => "FALLEN"u8,
			1 => "SWARMED"u8,
			2 => "IMPALED"u8,
			3 => "GORED"u8,
			4 => "INFESTED"u8,
			5 => "OPENED"u8,
			6 => "PURGED"u8,
			7 => "DESECRATED"u8,
			8 => "SACRIFICED"u8,
			9 => "EVISCERATED"u8,
			10 => "ANNIHILATED"u8,
			11 => "INTOXICATED"u8,
			12 => "ENVENOMATED"u8,
			13 => "INCARNATED"u8,
			14 => "DISCARNATED"u8,
			15 => "ENTANGLED"u8,
			16 => "HAUNTED"u8,
			_ => Inline.Utf8($"Invalid ({deathType})"),
		};
	}
}
