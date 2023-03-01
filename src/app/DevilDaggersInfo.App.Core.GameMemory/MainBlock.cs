#pragma warning disable IDE1006 // Naming Styles

using System.Text;

namespace DevilDaggersInfo.App.Core.GameMemory;

public readonly record struct MainBlock
{
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

	public readonly short Skull1sAlive;
	public readonly short Skull2sAlive;
	public readonly short Skull3sAlive;
	public readonly short SpiderlingsAlive;
	public readonly short Skull4sAlive;
	public readonly short Squid1sAlive;
	public readonly short Squid2sAlive;
	public readonly short Squid3sAlive;
	public readonly short CentipedesAlive;
	public readonly short GigapedesAlive;
	public readonly short Spider1sAlive;
	public readonly short Spider2sAlive;
	public readonly short LeviathansAlive;
	public readonly short OrbsAlive;
	public readonly short ThornsAlive;
	public readonly short GhostpedesAlive;
	public readonly short SpiderEggsAlive;

	public readonly short Skull1sKilled;
	public readonly short Skull2sKilled;
	public readonly short Skull3sKilled;
	public readonly short SpiderlingsKilled;
	public readonly short Skull4sKilled;
	public readonly short Squid1sKilled;
	public readonly short Squid2sKilled;
	public readonly short Squid3sKilled;
	public readonly short CentipedesKilled;
	public readonly short GigapedesKilled;
	public readonly short Spider1sKilled;
	public readonly short Spider2sKilled;
	public readonly short LeviathansKilled;
	public readonly short OrbsKilled;
	public readonly short ThornsKilled;
	public readonly short GhostpedesKilled;
	public readonly short SpiderEggsKilled;

	public readonly bool IsPlayerAlive;
	public readonly bool IsReplay;
	public readonly byte DeathType;
	public readonly bool IsInGame;

	public readonly int ReplayPlayerId;
	public readonly string ReplayPlayerName = string.Empty;

	public readonly byte[] SurvivalHashMd5 = Array.Empty<byte>();

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

		Skull1sAlive = br.ReadInt16();
		Skull2sAlive = br.ReadInt16();
		Skull3sAlive = br.ReadInt16();
		SpiderlingsAlive = br.ReadInt16();
		Skull4sAlive = br.ReadInt16();
		Squid1sAlive = br.ReadInt16();
		Squid2sAlive = br.ReadInt16();
		Squid3sAlive = br.ReadInt16();
		CentipedesAlive = br.ReadInt16();
		GigapedesAlive = br.ReadInt16();
		Spider1sAlive = br.ReadInt16();
		Spider2sAlive = br.ReadInt16();
		LeviathansAlive = br.ReadInt16();
		OrbsAlive = br.ReadInt16();
		ThornsAlive = br.ReadInt16();
		GhostpedesAlive = br.ReadInt16();
		SpiderEggsAlive = br.ReadInt16();

		Skull1sKilled = br.ReadInt16();
		Skull2sKilled = br.ReadInt16();
		Skull3sKilled = br.ReadInt16();
		SpiderlingsKilled = br.ReadInt16();
		Skull4sKilled = br.ReadInt16();
		Squid1sKilled = br.ReadInt16();
		Squid2sKilled = br.ReadInt16();
		Squid3sKilled = br.ReadInt16();
		CentipedesKilled = br.ReadInt16();
		GigapedesKilled = br.ReadInt16();
		Spider1sKilled = br.ReadInt16();
		Spider2sKilled = br.ReadInt16();
		LeviathansKilled = br.ReadInt16();
		OrbsKilled = br.ReadInt16();
		ThornsKilled = br.ReadInt16();
		GhostpedesKilled = br.ReadInt16();
		SpiderEggsKilled = br.ReadInt16();

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

		br.BaseStream.Seek(3, SeekOrigin.Current);

		ReplayBase = br.ReadInt64();
		ReplayLength = br.ReadInt32();

		PlayReplayFromMemory = br.ReadBoolean();
		GameMode = br.ReadByte();
		TimeAttackOrRaceFinished = br.ReadBoolean();
	}

	private static string Utf8StringFromBytes(byte[] bytes)
		=> Encoding.UTF8.GetString(bytes[..Array.IndexOf(bytes, (byte)0)]);
}
