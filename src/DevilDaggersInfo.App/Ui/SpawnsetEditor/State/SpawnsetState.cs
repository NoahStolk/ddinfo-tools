using DevilDaggersInfo.App.Ui.Popups;
using DevilDaggersInfo.Core.Spawnset;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.State;

public static class SpawnsetState
{
	public const string UntitledName = "<untitled>";

	private static SpawnsetBinary _spawnset;
	private static byte[] _memorySpawnsetMd5Hash;
	private static byte[] _fileSpawnsetMd5Hash;

	[SuppressMessage("Minor Code Smell", "S3963:\"static\" fields should be initialized inline", Justification = "Readability")]
	static SpawnsetState()
	{
		_spawnset = SpawnsetBinary.CreateDefault();

		byte[] spawnsetBytes = _spawnset.ToBytes();
		_memorySpawnsetMd5Hash = MD5.HashData(spawnsetBytes);
		_fileSpawnsetMd5Hash = MD5.HashData(spawnsetBytes);
	}

	public static SpawnsetBinary Spawnset
	{
		get => _spawnset;
		set
		{
			_spawnset = value;

			_memorySpawnsetMd5Hash = MD5.HashData(_spawnset.ToBytes());
			IsSpawnsetModified = !_fileSpawnsetMd5Hash.SequenceEqual(_memorySpawnsetMd5Hash);
		}
	}

	public static string? SpawnsetName { get; private set; }

	public static string? SpawnsetPath { get; private set; }

	public static bool IsSpawnsetModified { get; private set; }

	public static void SaveFile(string path)
	{
		File.WriteAllBytes(path, Spawnset.ToBytes());
		SetFile(path, Path.GetFileName(path));
	}

	public static void SetFile(string? path, string? name)
	{
		SpawnsetPath = path;
		SpawnsetName = name;
		_fileSpawnsetMd5Hash = _memorySpawnsetMd5Hash;
		IsSpawnsetModified = !_fileSpawnsetMd5Hash.SequenceEqual(_memorySpawnsetMd5Hash);
	}

	public static void PromptSaveSpawnset(Action action)
	{
		if (!IsSpawnsetModified)
			action();
		else
			PopupManager.ShowSaveSpawnsetPrompt(action);
	}
}
