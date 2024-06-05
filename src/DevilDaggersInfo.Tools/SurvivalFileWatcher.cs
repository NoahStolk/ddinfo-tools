using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.User.Settings;
using System.Net;
using System.Security.Cryptography;

namespace DevilDaggersInfo.Tools;

// TODO: This should be a class implementing IDisposable.
public static class SurvivalFileWatcher
{
#pragma warning disable S1450 // Cannot change this into a local. The events would not be raised.
	private static FileSystemWatcher? _survivalFileWatcher;
#pragma warning restore S1450

	public static bool Exists { get; private set; }

	public static string? SpawnsetName { get; private set; }

	public static HandLevel HandLevel { get; private set; } = HandLevel.Level1;
	public static int AdditionalGems { get; private set; }
	public static float TimerStart { get; private set; }
	public static EffectivePlayerSettings EffectivePlayerSettings { get; private set; } = new(HandLevel.Level1, 0, HandLevel.Level1);

	public static void Initialize()
	{
		UpdateActiveSpawnsetBasedOnHash();

		_survivalFileWatcher = new FileSystemWatcher(UserSettings.ModsDirectory, "survival");
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
			Exists = File.Exists(UserSettings.ModsSurvivalPath);

			if (!Exists)
			{
				SpawnsetName = null;
				return;
			}

			byte[] fileContents;
			byte[] fileHash;
			try
			{
				fileContents = File.ReadAllBytes(UserSettings.ModsSurvivalPath);
				fileHash = MD5.HashData(fileContents);
			}
			catch (Exception ex) when (ex.IsFileIoException())
			{
				Root.Log.Warning(ex, "Failed to update active spawnset based on hash.");
				return;
			}

			AsyncHandler.Run(
				getSpawnsetResult => getSpawnsetResult.Match(
					onSuccess: getSpawnset => SpawnsetName = getSpawnset.Name,
					onError: apiError =>
					{
						SpawnsetName = null;
						if (apiError.Exception is not HttpRequestException { StatusCode: HttpStatusCode.NotFound })
							Root.Log.Warning(apiError.Exception, "Failed to update active spawnset based on hash.");
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
}
