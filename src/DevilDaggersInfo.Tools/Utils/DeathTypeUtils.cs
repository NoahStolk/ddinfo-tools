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
}
