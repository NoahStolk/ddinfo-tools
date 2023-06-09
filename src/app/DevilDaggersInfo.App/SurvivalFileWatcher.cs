using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.Core.Spawnset;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App;

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

	public static void Initialize()
	{
		UpdateActiveSpawnsetBasedOnHash();

		_survivalFileWatcher = new(UserSettings.ModsDirectory, "survival");
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
			}
			else
			{
				try
				{
					byte[] fileContents = File.ReadAllBytes(UserSettings.ModsSurvivalPath);
					byte[] fileHash = MD5.HashData(fileContents);
					AsyncHandler.Run(s => SpawnsetName = s?.Name, () => FetchSpawnsetByHash.HandleAsync(fileHash));

					if (SpawnsetBinary.TryParse(fileContents, out SpawnsetBinary? spawnsetBinary))
					{
						HandLevel = spawnsetBinary.HandLevel;
						AdditionalGems = spawnsetBinary.AdditionalGems;
						TimerStart = spawnsetBinary.TimerStart;
					}
				}
				catch (Exception ex)
				{
					Root.Log.Warning(ex, "Failed to update active spawnset based on hash.");
				}
			}
		}
	}
}
