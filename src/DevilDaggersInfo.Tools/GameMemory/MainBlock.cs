#pragma warning disable IDE1006 // Naming Styles

using System.Buffers.Binary;
using System.Text;

namespace DevilDaggersInfo.Tools.GameMemory;

internal readonly record struct MainBlock
{
	/// <summary>
	/// The size of the whole block, in bytes.
	/// </summary>
	public const int Size = 319;

	private const int _formatVersionOffset = 12;
	private const int _playerNameOffset = 20;
	private const int _replayPlayerNameOffset = 176;
	private const int _nameFieldSize = 32;

	public readonly string Marker = string.Empty;
	public readonly int FormatVersion;

	public readonly int PlayerId;
	public readonly string PlayerName = string.Empty;
	public readonly float Time;
	public readonly int GemsCollected;
	public readonly int EnemiesKilled;
	public readonly int DaggersFired;
	public readonly int DaggersHit;
	public readonly int EnemiesAlive;
	public readonly int LevelGems;
	public readonly int HomingStored;
	public readonly int GemsDespawned;
	public readonly int GemsEaten;
	public readonly int GemsTotal;
	public readonly int HomingEaten;

	public readonly short Skull1AliveCount;
	public readonly short Skull2AliveCount;
	public readonly short Skull3AliveCount;
	public readonly short SpiderlingAliveCount;
	public readonly short Skull4AliveCount;
	public readonly short Squid1AliveCount;
	public readonly short Squid2AliveCount;
	public readonly short Squid3AliveCount;
	public readonly short CentipedeAliveCount;
	public readonly short GigapedeAliveCount;
	public readonly short Spider1AliveCount;
	public readonly short Spider2AliveCount;
	public readonly short LeviathanAliveCount;
	public readonly short OrbAliveCount;
	public readonly short ThornAliveCount;
	public readonly short GhostpedeAliveCount;
	public readonly short SpiderEggAliveCount;

	public readonly short Skull1KillCount;
	public readonly short Skull2KillCount;
	public readonly short Skull3KillCount;
	public readonly short SpiderlingKillCount;
	public readonly short Skull4KillCount;
	public readonly short Squid1KillCount;
	public readonly short Squid2KillCount;
	public readonly short Squid3KillCount;
	public readonly short CentipedeKillCount;
	public readonly short GigapedeKillCount;
	public readonly short Spider1KillCount;
	public readonly short Spider2KillCount;
	public readonly short LeviathanKillCount;
	public readonly short OrbKillCount;
	public readonly short ThornKillCount;
	public readonly short GhostpedeKillCount;
	public readonly short SpiderEggKillCount;

	public readonly bool IsPlayerAlive;
	public readonly bool IsReplay;
	public readonly byte DeathType;
	public readonly bool IsInGame;

	public readonly int ReplayPlayerId;
	public readonly string ReplayPlayerName = string.Empty;

	public readonly byte[] SurvivalHashMd5 = [];

	public readonly float LevelUpTime2;
	public readonly float LevelUpTime3;
	public readonly float LevelUpTime4;

	public readonly float LeviathanDownTime;
	public readonly float OrbDownTime;

	public readonly int Status;

	public readonly int HomingMax;
	public readonly float HomingMaxTime;
	public readonly int EnemiesAliveMax;
	public readonly float EnemiesAliveMaxTime;
	public readonly float MaxTime;

	public readonly long StatsBase;
	public readonly int StatsCount;
	public readonly bool StatsLoaded;

	public readonly int StartHandLevel;
	public readonly int StartAdditionalGems;
	public readonly float StartTimer;

	public readonly bool ProhibitedMods;

	public readonly long ReplayBase;
	public readonly int ReplayLength;

	public readonly bool PlayReplayFromMemory;
	public readonly byte GameMode;
	public readonly bool TimeAttackOrRaceFinished;

	public MainBlock(byte[] buffer)
	{
		using MemoryStream ms = new(buffer);
		using BinaryReader br = new(ms);
		Marker = Utf8StringFromBytes(br.ReadBytes(12));
		FormatVersion = br.ReadInt32();

		PlayerId = br.ReadInt32();
		PlayerName = Utf8StringFromBytes(br.ReadBytes(32));
		Time = br.ReadSingle();
		GemsCollected = br.ReadInt32();
		EnemiesKilled = br.ReadInt32();
		DaggersFired = br.ReadInt32();
		DaggersHit = br.ReadInt32();
		EnemiesAlive = br.ReadInt32();
		LevelGems = br.ReadInt32();
		HomingStored = br.ReadInt32();
		GemsDespawned = br.ReadInt32();
		GemsEaten = br.ReadInt32();
		GemsTotal = br.ReadInt32();
		HomingEaten = br.ReadInt32();

		Skull1AliveCount = br.ReadInt16();
		Skull2AliveCount = br.ReadInt16();
		Skull3AliveCount = br.ReadInt16();
		SpiderlingAliveCount = br.ReadInt16();
		Skull4AliveCount = br.ReadInt16();
		Squid1AliveCount = br.ReadInt16();
		Squid2AliveCount = br.ReadInt16();
		Squid3AliveCount = br.ReadInt16();
		CentipedeAliveCount = br.ReadInt16();
		GigapedeAliveCount = br.ReadInt16();
		Spider1AliveCount = br.ReadInt16();
		Spider2AliveCount = br.ReadInt16();
		LeviathanAliveCount = br.ReadInt16();
		OrbAliveCount = br.ReadInt16();
		ThornAliveCount = br.ReadInt16();
		GhostpedeAliveCount = br.ReadInt16();
		SpiderEggAliveCount = br.ReadInt16();

		Skull1KillCount = br.ReadInt16();
		Skull2KillCount = br.ReadInt16();
		Skull3KillCount = br.ReadInt16();
		SpiderlingKillCount = br.ReadInt16();
		Skull4KillCount = br.ReadInt16();
		Squid1KillCount = br.ReadInt16();
		Squid2KillCount = br.ReadInt16();
		Squid3KillCount = br.ReadInt16();
		CentipedeKillCount = br.ReadInt16();
		GigapedeKillCount = br.ReadInt16();
		Spider1KillCount = br.ReadInt16();
		Spider2KillCount = br.ReadInt16();
		LeviathanKillCount = br.ReadInt16();
		OrbKillCount = br.ReadInt16();
		ThornKillCount = br.ReadInt16();
		GhostpedeKillCount = br.ReadInt16();
		SpiderEggKillCount = br.ReadInt16();

		IsPlayerAlive = br.ReadBoolean();
		IsReplay = br.ReadBoolean();
		DeathType = br.ReadByte();
		IsInGame = br.ReadBoolean();

		ReplayPlayerId = br.ReadInt32();
		ReplayPlayerName = Utf8StringFromBytes(br.ReadBytes(32));

		SurvivalHashMd5 = br.ReadBytes(16);

		LevelUpTime2 = br.ReadSingle();
		LevelUpTime3 = br.ReadSingle();
		LevelUpTime4 = br.ReadSingle();

		LeviathanDownTime = br.ReadSingle();
		OrbDownTime = br.ReadSingle();

		Status = br.ReadInt32();

		HomingMax = br.ReadInt32();
		HomingMaxTime = br.ReadSingle();
		EnemiesAliveMax = br.ReadInt32();
		EnemiesAliveMaxTime = br.ReadSingle();
		MaxTime = br.ReadSingle();

		br.BaseStream.Seek(4, SeekOrigin.Current);

		StatsBase = br.ReadInt64();
		StatsCount = br.ReadInt32();
		StatsLoaded = br.ReadBoolean();

		br.BaseStream.Seek(3, SeekOrigin.Current);

		StartHandLevel = br.ReadInt32();
		StartAdditionalGems = br.ReadInt32();
		StartTimer = br.ReadSingle();

		ProhibitedMods = br.ReadBoolean();

		// Not available on Linux?
		br.BaseStream.Seek(3, SeekOrigin.Current);

		ReplayBase = br.ReadInt64();
		ReplayLength = br.ReadInt32();

		PlayReplayFromMemory = br.ReadBoolean();
		GameMode = br.ReadByte();
		TimeAttackOrRaceFinished = br.ReadBoolean();
	}

	/// <summary>
	/// The bytes the game writes at the start of the block. Platforms with no marker offset to fetch find the block by
	/// searching the game's memory for these.
	/// </summary>
	public static ReadOnlySpan<byte> MarkerBytes => "__ddstats__"u8;

	/// <summary>
	/// Whether <paramref name="buffer" /> holds a ddstats block, rather than one of the other copies of the marker
	/// bytes lying around in the process - the game's own string literal, for one.
	/// </summary>
	/// <remarks>
	/// This is only needed by platforms that find the block by searching for it; reading a pointer at the marker offset
	/// cannot land on the wrong candidate. It also guarantees the constructor can parse the buffer: the name fields are
	/// read up to their null terminator, and a buffer without one would throw.
	/// </remarks>
	public static bool IsValid(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < Size)
			return false;

		if (!buffer[..MarkerBytes.Length].SequenceEqual(MarkerBytes) || buffer[MarkerBytes.Length] != 0)
			return false;

		// Every build whose layout is known writes 1 here. Allow a bump so a future one is not rejected out of hand,
		// but not the arbitrary bytes that follow a stray copy of the marker.
		int formatVersion = BinaryPrimitives.ReadInt32LittleEndian(buffer[_formatVersionOffset..]);
		if (formatVersion is < 1 or > 1000)
			return false;

		return buffer.Slice(_playerNameOffset, _nameFieldSize).Contains((byte)0) && buffer.Slice(_replayPlayerNameOffset, _nameFieldSize).Contains((byte)0);
	}

	private static string Utf8StringFromBytes(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes[..Array.IndexOf(bytes, (byte)0)]);
	}

	public bool ArePlayersEqual(MainBlock other)
	{
		return IsReplay == other.IsReplay && (IsReplay ? ReplayPlayerId == other.ReplayPlayerId : PlayerId == other.PlayerId);
	}
}
