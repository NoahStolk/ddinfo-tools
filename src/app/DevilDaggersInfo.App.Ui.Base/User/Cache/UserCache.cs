using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.User.Cache.Model;
using System.Text.Json;
using Warp.NET.Debugging;

namespace DevilDaggersInfo.App.Ui.Base.User.Cache;

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
			_model = value;
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
					_model = deserializedModel;
			}
			catch (Exception ex)
			{
				Root.Dependencies.Log.Error(ex, "Failed to load user cache.");
			}
		}

		StateManager.Dispatch(new UserSettingsLoaded());
	}

	private static void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model));

		DebugStack.Add("Saved user cache.", 1);
	}
}
