using DevilDaggersInfo.App.User.Cache.Model;
using System.Text.Json;

namespace DevilDaggersInfo.App.User.Cache;

public static class UserCache
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ddinfo-tools");
	private static readonly string _filePath = Path.Combine(_fileDirectory, "cache");

	private static UserCacheModel _model = UserCacheModel.Default;

	public static UserCacheModel Model
	{
		get => _model;
		set
		{
			_model = value.Sanitize();
			Save();
		}
	}

	public static void Load()
	{
		if (File.Exists(_filePath))
		{
			try
			{
				UserCacheModel? deserializedModel = JsonSerializer.Deserialize<UserCacheModel>(File.ReadAllText(_filePath));
				if (deserializedModel != null)
					_model = deserializedModel.Sanitize();
			}
			catch (Exception ex)
			{
				Root.Log.Error(ex, "Failed to load user cache.");
			}
		}
	}

	private static void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model));
	}
}
