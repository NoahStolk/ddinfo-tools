using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;
using DevilDaggersInfo.App.Ui.Base.User.Cache;
using DevilDaggersInfo.Common.Extensions;
using DevilDaggersInfo.Core.Encryption;
#if !SKIP_VALUE
using System.IO.Compression;
using System.Text;
#endif
using System.Web;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder;

public static class RecordingLogic
{
	private static readonly AesBase32Wrapper _aesBase32Wrapper;
	private static MainBlock? _runToUpload;

#pragma warning disable S3963
	static RecordingLogic()
#pragma warning restore S3963
	{
#if SKIP_VALUE
		_aesBase32Wrapper = new(string.Empty, string.Empty, string.Empty);
#else
		using MemoryStream msIn = new(WarpBlobs.Value.Data.Select((b, i) => i < 4 ? (byte)(b << 4 | b >> 4) : (byte)~b).ToArray());
		using MemoryStream msOut = new();
		using DeflateStream ds = new(msIn, CompressionMode.Decompress);
		ds.CopyTo(msOut);
		using BinaryReader br = new(msOut);
		br.BaseStream.Position = 0;
		string[] values = Enumerable.Range(0, br.ReadByte()).Select(_ => Encoding.UTF8.GetString(br.ReadBytes(br.ReadByte()).Select((b, j) => j % 4 == 0 ? b : (byte)~b).ToArray())).ToArray();
		_aesBase32Wrapper = new(values[0], values[1], values[2]);
#endif
	}

	/// <summary>
	/// Scans game memory. If the marker is not known, fires the call to retrieve it, then returns false because memory can't be scanned until the HTTP call has returned successfully.
	/// </summary>
	/// <returns>Whether the marker is known.</returns>
	public static bool Scan()
	{
		if (!StateManager.MarkerState.Marker.HasValue)
		{
			InitializeMarker();
			return false;
		}

		// Always initialize the process so we detach properly when the game exits.
		Root.Dependencies.GameMemoryService.Initialize(StateManager.MarkerState.Marker.Value);
		Root.Dependencies.GameMemoryService.Scan();

		return true;
	}

	public static void Handle()
	{
		// Do not execute if the game is not running.
		if (!Root.Dependencies.GameMemoryService.IsInitialized)
		{
			StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForGame));
			return;
		}

		MainBlock mainBlock = Root.Dependencies.GameMemoryService.MainBlock;
		MainBlock mainBlockPrevious = Root.Dependencies.GameMemoryService.MainBlockPrevious;

		// When a run is scheduled to upload, keep trying until stats have loaded and the replay is valid.
		if (_runToUpload != null)
		{
			StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForStats));
			if (!mainBlock.StatsLoaded)
				return;

			StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForReplay));
			if (!Root.Dependencies.GameMemoryService.IsReplayValid())
				return;

			UploadRunIfExists(_runToUpload.Value);
			_runToUpload = null;
			return;
		}

		// Set current player ID when it has not been set yet.
		// When the game starts up it will be set to -1, and then to the player ID.
		if (UserCache.Model.PlayerId == 0 && mainBlock.PlayerId > 0)
		{
			StateManager.Dispatch(new SetCurrentPlayerId(mainBlock.PlayerId));
		}

		// Indicate recording status.
		GameStatus status = (GameStatus)mainBlock.Status;
		if (StateManager.RecordingState.RecordingStateType != RecordingStateType.Recording)
		{
			if (status is GameStatus.Title or GameStatus.Menu or GameStatus.Lobby || Math.Abs(mainBlock.Time - mainBlockPrevious.Time) < 0.0001f)
			{
				StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForNextRun));
				return;
			}

			StateManager.Dispatch(new SetRecordingState(RecordingStateType.Recording));
		}

#if !FORCE_LOCAL_REPLAYS
		if (status == GameStatus.LocalReplay)
		{
			StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForLocalReplay));
			return;
		}
#endif

		if (status == GameStatus.OwnReplayFromLeaderboard)
		{
			StateManager.Dispatch(new SetRecordingState(RecordingStateType.WaitingForLeaderboardReplay));
			return;
		}

		// Determine whether to upload the run or not.
		bool justDied = !mainBlock.IsPlayerAlive && mainBlockPrevious.IsPlayerAlive;
		if (justDied && (mainBlock.GameMode == 0 || mainBlock.TimeAttackOrRaceFinished))
			_runToUpload = mainBlock;
	}

	private static void InitializeMarker()
	{
		AsyncHandler.Run(SetMarker, () => FetchMarker.HandleAsync(Root.Dependencies.PlatformSpecificValues.OperatingSystem));

		void SetMarker(GetMarker? getMarker)
		{
			if (getMarker == null)
				Root.Dependencies.NativeDialogService.ReportError("Failed to retrieve marker.");
			else
				StateManager.Dispatch(new SetMarker(getMarker.Value));
		}
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

		GameMemoryService memoryService = Root.Dependencies.GameMemoryService;
		byte[] statsBuffer = memoryService.GetStatsBuffer();

		AddUploadRequest uploadRequest = new()
		{
			DaggersFired = runToUpload.DaggersFired,
			DaggersHit = runToUpload.DaggersHit,
			ClientVersion = Root.Game.AppVersion.ToString(),
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
			OperatingSystem = Root.Dependencies.PlatformSpecificValues.OperatingSystem.ToString(),
			ProhibitedMods = runToUpload.ProhibitedMods,
			Client = "ddinfo-tools",
			ReplayData = memoryService.ReadReplayFromMemory(),
#if FORCE_LOCAL_REPLAYS
			Status = (int)GameStatus.Dead,
#else
			Status = runToUpload.Status,
#endif
			ReplayPlayerId = runToUpload.ReplayPlayerId,
			GameMode = runToUpload.GameMode,
			TimeAttackOrRaceFinished = runToUpload.TimeAttackOrRaceFinished,
		};

		AsyncHandler.Run(OnSubmit, () => UploadSubmission.HandleAsync(uploadRequest));
	}

	private static void OnSubmit(GetUploadResponse? response)
	{
		if (response == null)
		{
			Root.Dependencies.NativeDialogService.ReportError("Failed to upload run.");
			return;
		}

		StateManager.Dispatch(new SetSuccessfulUpload(response));
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
		List<ushort> skull1SAlive = new();
		List<ushort> skull2SAlive = new();
		List<ushort> skull3SAlive = new();
		List<ushort> spiderlingsAlive = new();
		List<ushort> skull4SAlive = new();
		List<ushort> squid1SAlive = new();
		List<ushort> squid2SAlive = new();
		List<ushort> squid3SAlive = new();
		List<ushort> centipedesAlive = new();
		List<ushort> gigapedesAlive = new();
		List<ushort> spider1SAlive = new();
		List<ushort> spider2SAlive = new();
		List<ushort> leviathansAlive = new();
		List<ushort> orbsAlive = new();
		List<ushort> thornsAlive = new();
		List<ushort> ghostpedesAlive = new();
		List<ushort> spiderEggsAlive = new();
		List<ushort> skull1SKilled = new();
		List<ushort> skull2SKilled = new();
		List<ushort> skull3SKilled = new();
		List<ushort> spiderlingsKilled = new();
		List<ushort> skull4SKilled = new();
		List<ushort> squid1SKilled = new();
		List<ushort> squid2SKilled = new();
		List<ushort> squid3SKilled = new();
		List<ushort> centipedesKilled = new();
		List<ushort> gigapedesKilled = new();
		List<ushort> spider1SKilled = new();
		List<ushort> spider2SKilled = new();
		List<ushort> leviathansKilled = new();
		List<ushort> orbsKilled = new();
		List<ushort> thornsKilled = new();
		List<ushort> ghostpedesKilled = new();
		List<ushort> spiderEggsKilled = new();

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

			skull1SAlive.Add(br.ReadUInt16());
			skull2SAlive.Add(br.ReadUInt16());
			skull3SAlive.Add(br.ReadUInt16());
			spiderlingsAlive.Add(br.ReadUInt16());
			skull4SAlive.Add(br.ReadUInt16());
			squid1SAlive.Add(br.ReadUInt16());
			squid2SAlive.Add(br.ReadUInt16());
			squid3SAlive.Add(br.ReadUInt16());
			centipedesAlive.Add(br.ReadUInt16());
			gigapedesAlive.Add(br.ReadUInt16());
			spider1SAlive.Add(br.ReadUInt16());
			spider2SAlive.Add(br.ReadUInt16());
			leviathansAlive.Add(br.ReadUInt16());
			orbsAlive.Add(br.ReadUInt16());
			thornsAlive.Add(br.ReadUInt16());
			ghostpedesAlive.Add(br.ReadUInt16());
			spiderEggsAlive.Add(br.ReadUInt16());

			skull1SKilled.Add(br.ReadUInt16());
			skull2SKilled.Add(br.ReadUInt16());
			skull3SKilled.Add(br.ReadUInt16());
			spiderlingsKilled.Add(br.ReadUInt16());
			skull4SKilled.Add(br.ReadUInt16());
			squid1SKilled.Add(br.ReadUInt16());
			squid2SKilled.Add(br.ReadUInt16());
			squid3SKilled.Add(br.ReadUInt16());
			centipedesKilled.Add(br.ReadUInt16());
			gigapedesKilled.Add(br.ReadUInt16());
			spider1SKilled.Add(br.ReadUInt16());
			spider2SKilled.Add(br.ReadUInt16());
			leviathansKilled.Add(br.ReadUInt16());
			orbsKilled.Add(br.ReadUInt16());
			thornsKilled.Add(br.ReadUInt16());
			ghostpedesKilled.Add(br.ReadUInt16());
			spiderEggsKilled.Add(br.ReadUInt16());
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
			Skull1sAlive = skull1SAlive,
			Skull2sAlive = skull2SAlive,
			Skull3sAlive = skull3SAlive,
			SpiderlingsAlive = spiderlingsAlive,
			Skull4sAlive = skull4SAlive,
			Squid1sAlive = squid1SAlive,
			Squid2sAlive = squid2SAlive,
			Squid3sAlive = squid3SAlive,
			CentipedesAlive = centipedesAlive,
			GigapedesAlive = gigapedesAlive,
			Spider1sAlive = spider1SAlive,
			Spider2sAlive = spider2SAlive,
			LeviathansAlive = leviathansAlive,
			OrbsAlive = orbsAlive,
			ThornsAlive = thornsAlive,
			GhostpedesAlive = ghostpedesAlive,
			SpiderEggsAlive = spiderEggsAlive,
			Skull1sKilled = skull1SKilled,
			Skull2sKilled = skull2SKilled,
			Skull3sKilled = skull3SKilled,
			SpiderlingsKilled = spiderlingsKilled,
			Skull4sKilled = skull4SKilled,
			Squid1sKilled = squid1SKilled,
			Squid2sKilled = squid2SKilled,
			Squid3sKilled = squid3SKilled,
			CentipedesKilled = centipedesKilled,
			GigapedesKilled = gigapedesKilled,
			Spider1sKilled = spider1SKilled,
			Spider2sKilled = spider2SKilled,
			LeviathansKilled = leviathansKilled,
			OrbsKilled = orbsKilled,
			ThornsKilled = thornsKilled,
			GhostpedesKilled = ghostpedesKilled,
			SpiderEggsKilled = spiderEggsKilled,
		};
	}
}
