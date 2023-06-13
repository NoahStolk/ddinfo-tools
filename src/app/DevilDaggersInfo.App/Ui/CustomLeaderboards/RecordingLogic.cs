using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.App.User.Cache;
using DevilDaggersInfo.Common.Extensions;
using DevilDaggersInfo.Core.Encryption;
#if !SKIP_VALUE
using System.IO.Compression;
using System.Text;
#endif
using System.Web;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards;

public static class RecordingLogic
{
	private static readonly AesBase32Wrapper _aesBase32Wrapper;
	private static MainBlock? _runToUpload;

	private static readonly List<AddUploadRequestTimestamp> _timestamps = new();

#pragma warning disable S3963
	static RecordingLogic()
#pragma warning restore S3963
	{
#if SKIP_VALUE
		_aesBase32Wrapper = new(string.Empty, string.Empty, string.Empty);
#else
		using MemoryStream msIn = new(Root.InternalResources.Value.Data.Select((b, i) => i < 4 ? (byte)(b << 4 | b >> 4) : (byte)~b).ToArray());
		using MemoryStream msOut = new();
		using DeflateStream ds = new(msIn, CompressionMode.Decompress);
		ds.CopyTo(msOut);
		using BinaryReader br = new(msOut);
		br.BaseStream.Position = 0;
		string[] values = Enumerable.Range(0, br.ReadByte()).Select(_ => Encoding.UTF8.GetString(br.ReadBytes(br.ReadByte()).Select((b, j) => j % 4 == 0 ? b : (byte)~b).ToArray())).ToArray();
		_aesBase32Wrapper = new(values[0], values[1], values[2]);
#endif
	}

	public static RecordingStateType RecordingStateType { get; private set; }
	public static DateTime? LastSubmission { get; private set; }

	// TODO: Flip values and rename to ShowRecording.
	public static bool ShowUploadResponse { get; private set; }

	public static IEnumerable<AddUploadRequestTimestamp> Timestamps => _timestamps;

	public static void Handle()
	{
		// Do not execute if the game is not running.
		if (!Root.GameMemoryService.IsInitialized)
		{
			RecordingStateType = RecordingStateType.WaitingForGame;
			return;
		}

		MainBlock mainBlock = Root.GameMemoryService.MainBlock;
		MainBlock mainBlockPrevious = Root.GameMemoryService.MainBlockPrevious;

		// When a run is scheduled to upload, keep trying until stats have loaded and the replay is valid.
		if (_runToUpload != null)
		{
			RecordingStateType = RecordingStateType.WaitingForStats;
			if (!mainBlock.StatsLoaded)
				return;

			RecordingStateType = RecordingStateType.WaitingForReplay;
			if (!Root.GameMemoryService.IsReplayValid())
				return;

			UploadRunIfExists(_runToUpload.Value);
			_runToUpload = null;
			return;
		}

		// Set current player ID when it has not been set yet.
		// When the game starts up it will be set to -1, and then to the player ID.
		if (mainBlock.PlayerId > 0 && UserCache.Model.PlayerId != mainBlock.PlayerId)
		{
			UserCache.Model = UserCache.Model with { PlayerId = mainBlock.PlayerId };
		}

		// Indicate recording status.
		GameStatus status = (GameStatus)mainBlock.Status;
		if (RecordingStateType != RecordingStateType.Recording)
		{
			if (status is GameStatus.Title or GameStatus.Menu or GameStatus.Lobby || Math.Abs(mainBlock.Time - mainBlockPrevious.Time) < 0.0001f)
			{
				RecordingStateType = RecordingStateType.WaitingForNextRun;
				return;
			}

			RecordingStateType = RecordingStateType.Recording;
			ShowUploadResponse = false;
		}

#if !FORCE_LOCAL_REPLAYS
		if (status == GameStatus.LocalReplay)
		{
			RecordingStateType = RecordingStateType.WaitingForLocalReplay;
			return;
		}
#endif

		if (status == GameStatus.OwnReplayFromLeaderboard)
		{
			RecordingStateType = RecordingStateType.WaitingForLeaderboardReplay;
			return;
		}

		if (mainBlock.Time < mainBlockPrevious.Time)
		{
			_timestamps.Clear();
			AddTimestamp(mainBlock);
		}

		int expectedTimestampCount = (int)Math.Floor(mainBlock.Time / 60f) + 1;
		if (_timestamps.Count < expectedTimestampCount)
		{
			AddTimestamp(mainBlock);
			return;
		}

		// Determine whether to upload the run or not.
		bool justDied = !mainBlock.IsPlayerAlive && mainBlockPrevious.IsPlayerAlive;
		if (justDied && (mainBlock.GameMode == 0 || mainBlock.TimeAttackOrRaceFinished))
		{
			_runToUpload = mainBlock;
			AddTimestamp(mainBlock);
		}
	}

	private static void AddTimestamp(MainBlock mainBlock)
	{
		_timestamps.Add(new AddUploadRequestTimestamp
		{
			Timestamp = DateTime.UtcNow.Ticks,
			TimeInSeconds = mainBlock.Time,
		});
	}

	private static void UploadRunIfExists(MainBlock runToUpload)
	{
		AsyncHandler.Run(leaderboardExists => UploadRun(leaderboardExists, runToUpload), () => CheckIfLeaderboardExists.HandleAsync(runToUpload.SurvivalHashMd5));
	}

	private static void UploadRun(bool leaderboardExists, MainBlock runToUpload)
	{
		if (!leaderboardExists)
			return;

		byte[] timeAsBytes = BitConverter.GetBytes(runToUpload.Time);
		byte[] levelUpTime2AsBytes = BitConverter.GetBytes(runToUpload.LevelUpTime2);
		byte[] levelUpTime3AsBytes = BitConverter.GetBytes(runToUpload.LevelUpTime3);
		byte[] levelUpTime4AsBytes = BitConverter.GetBytes(runToUpload.LevelUpTime4);

		string toEncrypt = string.Join(
			";",
#if FORCE_LOCAL_REPLAYS
			runToUpload.ReplayPlayerId,
#else
			runToUpload.PlayerId,
#endif
			timeAsBytes.ByteArrayToHexString(),
			runToUpload.GemsCollected,
			runToUpload.GemsDespawned,
			runToUpload.GemsEaten,
			runToUpload.GemsTotal,
			runToUpload.EnemiesAlive,
			runToUpload.EnemiesKilled,
			runToUpload.DeathType,
			runToUpload.DaggersHit,
			runToUpload.DaggersFired,
			runToUpload.HomingStored,
			runToUpload.HomingEaten,
			runToUpload.IsReplay,
#if FORCE_LOCAL_REPLAYS
			(int)GameStatus.Dead,
#else
			runToUpload.Status,
#endif
			runToUpload.SurvivalHashMd5.ByteArrayToHexString(),
			levelUpTime2AsBytes.ByteArrayToHexString(),
			levelUpTime3AsBytes.ByteArrayToHexString(),
			levelUpTime4AsBytes.ByteArrayToHexString(),
			runToUpload.GameMode,
			runToUpload.TimeAttackOrRaceFinished,
			runToUpload.ProhibitedMods);
		string validation = _aesBase32Wrapper.EncryptAndEncode(toEncrypt);

		byte[] statsBuffer = Root.GameMemoryService.GetStatsBuffer();

		AddUploadRequest uploadRequest = new()
		{
			DaggersFired = runToUpload.DaggersFired,
			DaggersHit = runToUpload.DaggersHit,
			ClientVersion = Root.Application.AppVersion.ToString(),
			DeathType = runToUpload.DeathType,
			EnemiesAlive = runToUpload.EnemiesAlive,
			GemsCollected = runToUpload.GemsCollected,
			GemsDespawned = runToUpload.GemsDespawned,
			GemsEaten = runToUpload.GemsEaten,
			GemsTotal = runToUpload.GemsTotal,
			HomingStored = runToUpload.HomingStored,
			HomingEaten = runToUpload.HomingEaten,
			EnemiesKilled = runToUpload.EnemiesKilled,
			LevelUpTime2InSeconds = runToUpload.LevelUpTime2,
			LevelUpTime3InSeconds = runToUpload.LevelUpTime3,
			LevelUpTime4InSeconds = runToUpload.LevelUpTime4,
			LevelUpTime2AsBytes = levelUpTime2AsBytes,
			LevelUpTime3AsBytes = levelUpTime3AsBytes,
			LevelUpTime4AsBytes = levelUpTime4AsBytes,
#if FORCE_LOCAL_REPLAYS
			PlayerId = runToUpload.ReplayPlayerId,
#else
			PlayerId = runToUpload.PlayerId,
#endif
			SurvivalHashMd5 = runToUpload.SurvivalHashMd5,
			TimeInSeconds = runToUpload.Time,
			TimeAsBytes = timeAsBytes,
#if FORCE_LOCAL_REPLAYS
			PlayerName = runToUpload.ReplayPlayerName,
#else
			PlayerName = runToUpload.PlayerName,
#endif
			IsReplay = runToUpload.IsReplay,
			Validation = HttpUtility.HtmlEncode(validation),
			ValidationVersion = 2,
			GameData = GetGameDataForUpload(runToUpload, statsBuffer),
#if DEBUG
			BuildMode = "DEBUG",
#else
			BuildMode = "RELEASE",
#endif
			OperatingSystem = Root.PlatformSpecificValues.AppOperatingSystem.ToString(),
			ProhibitedMods = runToUpload.ProhibitedMods,
			Client = "ddinfo-tools",
			ReplayData = Root.GameMemoryService.ReadReplayFromMemory(),
#if FORCE_LOCAL_REPLAYS
			Status = (int)GameStatus.Dead,
#else
			Status = runToUpload.Status,
#endif
			ReplayPlayerId = runToUpload.ReplayPlayerId,
			GameMode = runToUpload.GameMode,
			TimeAttackOrRaceFinished = runToUpload.TimeAttackOrRaceFinished,
			Timestamps = _timestamps,
		};

		AsyncHandler.Run(uploadResponse => OnSubmit(uploadResponse, uploadRequest), () => UploadSubmission.HandleAsync(uploadRequest));
	}

	private static void OnSubmit(GetUploadResponse? response, AddUploadRequest uploadRequest)
	{
		if (response == null)
		{
			Modals.ShowError("Failed to upload run.");
			return;
		}

		ShowUploadResponse = true;
		LastSubmission = DateTime.Now;

		UploadResult uploadResult = new(response, response.IsAscending, response.SpawnsetName, uploadRequest.DeathType, DateTime.Now);
		CustomLeaderboardResultsWindow.AddResult(uploadResult);
	}

	private static AddGameData GetGameDataForUpload(MainBlock block, byte[] statsBuffer)
	{
		List<int> gemsCollected = new();
		List<int> enemiesKilled = new();
		List<int> daggersFired = new();
		List<int> daggersHit = new();
		List<int> enemiesAlive = new();
		List<int> homingStored = new();
		List<int> gemsDespawned = new();
		List<int> gemsEaten = new();
		List<int> gemsTotal = new();
		List<int> homingEaten = new();
		List<ushort> skull1AliveCount = new();
		List<ushort> skull2AliveCount = new();
		List<ushort> skull3AliveCount = new();
		List<ushort> spiderlingAliveCount = new();
		List<ushort> skull4AliveCount = new();
		List<ushort> squid1AliveCount = new();
		List<ushort> squid2AliveCount = new();
		List<ushort> squid3AliveCount = new();
		List<ushort> centipedeAliveCount = new();
		List<ushort> gigapedeAliveCount = new();
		List<ushort> spider1AliveCount = new();
		List<ushort> spider2AliveCount = new();
		List<ushort> leviathanAliveCount = new();
		List<ushort> orbAliveCount = new();
		List<ushort> thornAliveCount = new();
		List<ushort> ghostpedeAliveCount = new();
		List<ushort> spiderEggAliveCount = new();
		List<ushort> skull1KillCount = new();
		List<ushort> skull2KillCount = new();
		List<ushort> skull3KillCount = new();
		List<ushort> spiderlingKillCount = new();
		List<ushort> skull4KillCount = new();
		List<ushort> squid1KillCount = new();
		List<ushort> squid2KillCount = new();
		List<ushort> squid3KillCount = new();
		List<ushort> centipedeKillCount = new();
		List<ushort> gigapedeKillCount = new();
		List<ushort> spider1KillCount = new();
		List<ushort> spider2KillCount = new();
		List<ushort> leviathanKillCount = new();
		List<ushort> orbKillCount = new();
		List<ushort> thornKillCount = new();
		List<ushort> ghostpedeKillCount = new();
		List<ushort> spiderEggKillCount = new();

		using MemoryStream ms = new(statsBuffer);
		using BinaryReader br = new(ms);
		for (int i = 0; i < block.StatsCount; i++)
		{
			gemsCollected.Add(br.ReadInt32());
			enemiesKilled.Add(br.ReadInt32());
			daggersFired.Add(br.ReadInt32());
			daggersHit.Add(br.ReadInt32());
			enemiesAlive.Add(br.ReadInt32());
			_ = br.ReadInt32(); // Skip level gems.
			homingStored.Add(br.ReadInt32());
			gemsDespawned.Add(br.ReadInt32());
			gemsEaten.Add(br.ReadInt32());
			gemsTotal.Add(br.ReadInt32());
			homingEaten.Add(br.ReadInt32());

			skull1AliveCount.Add(br.ReadUInt16());
			skull2AliveCount.Add(br.ReadUInt16());
			skull3AliveCount.Add(br.ReadUInt16());
			spiderlingAliveCount.Add(br.ReadUInt16());
			skull4AliveCount.Add(br.ReadUInt16());
			squid1AliveCount.Add(br.ReadUInt16());
			squid2AliveCount.Add(br.ReadUInt16());
			squid3AliveCount.Add(br.ReadUInt16());
			centipedeAliveCount.Add(br.ReadUInt16());
			gigapedeAliveCount.Add(br.ReadUInt16());
			spider1AliveCount.Add(br.ReadUInt16());
			spider2AliveCount.Add(br.ReadUInt16());
			leviathanAliveCount.Add(br.ReadUInt16());
			orbAliveCount.Add(br.ReadUInt16());
			thornAliveCount.Add(br.ReadUInt16());
			ghostpedeAliveCount.Add(br.ReadUInt16());
			spiderEggAliveCount.Add(br.ReadUInt16());

			skull1KillCount.Add(br.ReadUInt16());
			skull2KillCount.Add(br.ReadUInt16());
			skull3KillCount.Add(br.ReadUInt16());
			spiderlingKillCount.Add(br.ReadUInt16());
			skull4KillCount.Add(br.ReadUInt16());
			squid1KillCount.Add(br.ReadUInt16());
			squid2KillCount.Add(br.ReadUInt16());
			squid3KillCount.Add(br.ReadUInt16());
			centipedeKillCount.Add(br.ReadUInt16());
			gigapedeKillCount.Add(br.ReadUInt16());
			spider1KillCount.Add(br.ReadUInt16());
			spider2KillCount.Add(br.ReadUInt16());
			leviathanKillCount.Add(br.ReadUInt16());
			orbKillCount.Add(br.ReadUInt16());
			thornKillCount.Add(br.ReadUInt16());
			ghostpedeKillCount.Add(br.ReadUInt16());
			spiderEggKillCount.Add(br.ReadUInt16());
		}

		return new()
		{
			GemsCollected = gemsCollected,
			EnemiesKilled = enemiesKilled,
			DaggersFired = daggersFired,
			DaggersHit = daggersHit,
			EnemiesAlive = enemiesAlive,
			HomingStored = homingStored,
			GemsDespawned = gemsDespawned,
			GemsEaten = gemsEaten,
			GemsTotal = gemsTotal,
			HomingEaten = homingEaten,
			Skull1sAlive = skull1AliveCount,
			Skull2sAlive = skull2AliveCount,
			Skull3sAlive = skull3AliveCount,
			SpiderlingsAlive = spiderlingAliveCount,
			Skull4sAlive = skull4AliveCount,
			Squid1sAlive = squid1AliveCount,
			Squid2sAlive = squid2AliveCount,
			Squid3sAlive = squid3AliveCount,
			CentipedesAlive = centipedeAliveCount,
			GigapedesAlive = gigapedeAliveCount,
			Spider1sAlive = spider1AliveCount,
			Spider2sAlive = spider2AliveCount,
			LeviathansAlive = leviathanAliveCount,
			OrbsAlive = orbAliveCount,
			ThornsAlive = thornAliveCount,
			GhostpedesAlive = ghostpedeAliveCount,
			SpiderEggsAlive = spiderEggAliveCount,
			Skull1sKilled = skull1KillCount,
			Skull2sKilled = skull2KillCount,
			Skull3sKilled = skull3KillCount,
			SpiderlingsKilled = spiderlingKillCount,
			Skull4sKilled = skull4KillCount,
			Squid1sKilled = squid1KillCount,
			Squid2sKilled = squid2KillCount,
			Squid3sKilled = squid3KillCount,
			CentipedesKilled = centipedeKillCount,
			GigapedesKilled = gigapedeKillCount,
			Spider1sKilled = spider1KillCount,
			Spider2sKilled = spider2KillCount,
			LeviathansKilled = leviathanKillCount,
			OrbsKilled = orbKillCount,
			ThornsKilled = thornKillCount,
			GhostpedesKilled = ghostpedeKillCount,
			SpiderEggsKilled = spiderEggKillCount,
		};
	}
}
