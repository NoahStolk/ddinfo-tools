using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.User.Settings;
using Serilog.Core;
using System.Net;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools;

internal sealed class SurvivalFileWatcher(Logger logger, UserSettings userSettings) : IDisposable
{
	private FileSystemWatcher? _survivalFileWatcher;

	public bool Exists { get; private set; }

	public string? SpawnsetName { get; private set; }

	public HandLevel HandLevel { get; private set; } = HandLevel.Level1;
	public int AdditionalGems { get; private set; }
	public float TimerStart { get; private set; }
	public EffectivePlayerSettings EffectivePlayerSettings { get; private set; } = new(HandLevel.Level1, 0, HandLevel.Level1);

	public void Initialize()
	{
		UpdateActiveSpawnsetBasedOnHash();

		_survivalFileWatcher = new FileSystemWatcher(userSettings.ModsDirectory, "survival");
		_survivalFileWatcher.NotifyFilter = NotifyFilters.CreationTime
			| NotifyFilters.DirectoryName
			| NotifyFilters.FileName
			| NotifyFilters.LastWrite
			| NotifyFilters.Size;
		_survivalFileWatcher.IncludeSubdirectories = true; // This needs to be enabled for some reason.
		_survivalFileWatcher.EnableRaisingEvents = true;
		_survivalFileWatcher.Changed += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Deleted += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Created += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Renamed += (_, _) => UpdateActiveSpawnsetBasedOnHash();
		_survivalFileWatcher.Error += (_, _) => UpdateActiveSpawnsetBasedOnHash();

		void UpdateActiveSpawnsetBasedOnHash()
		{
			Exists = File.Exists(userSettings.ModsSurvivalPath);

			if (!Exists)
			{
				SpawnsetName = null;
				return;
			}

			byte[] fileContents;
			byte[] fileHash;
			try
			{
				fileContents = File.ReadAllBytes(userSettings.ModsSurvivalPath);
				fileHash = MD5.HashData(fileContents);
			}
			catch (Exception ex) when (ex.IsFileIoException())
			{
				logger.Warning(ex, "Failed to update active spawnset based on hash.");
				return;
			}

			AsyncHandler.Run(
				getSpawnsetResult => getSpawnsetResult.Match(
					onSuccess: getSpawnset => SpawnsetName = getSpawnset.Name,
					onError: apiError =>
					{
						SpawnsetName = null;
						if (apiError.Exception is not HttpRequestException { StatusCode: HttpStatusCode.NotFound })
							logger.Warning(apiError.Exception, "Failed to update active spawnset based on hash.");
					}),
				() => FetchSpawnsetByHash.HandleAsync(fileHash));

			if (SpawnsetBinary.TryParse(fileContents, out SpawnsetBinary? spawnsetBinary))
			{
				HandLevel = spawnsetBinary.HandLevel;
				AdditionalGems = spawnsetBinary.AdditionalGems;
				TimerStart = spawnsetBinary.TimerStart;
				EffectivePlayerSettings = spawnsetBinary.GetEffectivePlayerSettings();
			}
		}
	}

	public void Dispose()
	{
		_survivalFileWatcher?.Dispose();
	}
}
