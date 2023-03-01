using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Settings.Model;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using System.Text.Json;
using Warp.NET.Debugging;

namespace DevilDaggersInfo.App.Ui.Base.Settings;

public static class UserSettings
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ddinfo-tools");
	private static readonly string _filePath = Path.Combine(_fileDirectory, "settings");

	private static UserSettingsModel _model = UserSettingsModel.Default;

	public static UserSettingsModel Model
	{
		get => _model;
		set
		{
			_model = value;
			Save();

			Root.Game.MainLoopRate = _model.MaxFps;
		}
	}

	public static string ModsDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "mods");

	public static string DdDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "dd");

	public static string ResDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "res");

	public static string ModsSurvivalPath => Path.Combine(ModsDirectory, "survival");

	public static string DdSurvivalPath => Path.Combine(DdDirectory, "survival");

	public static string ResAudioPath => Path.Combine(ResDirectory, "audio");

	public static string ResDdPath => Path.Combine(ResDirectory, "dd");

	public static void Load()
	{
		if (File.Exists(_filePath))
		{
			try
			{
				UserSettingsModel? deserializedModel = JsonSerializer.Deserialize<UserSettingsModel>(File.ReadAllText(_filePath));
				if (deserializedModel != null)
				{
					_model = deserializedModel with
					{
						MaxFps = Math.Clamp(_model.MaxFps, UserSettingsModel.MaxFpsMin, UserSettingsModel.MaxFpsMax),
						LookSpeed = Math.Clamp(_model.LookSpeed, UserSettingsModel.LookSpeedMin, UserSettingsModel.LookSpeedMax),
						FieldOfView = Math.Clamp(_model.FieldOfView, UserSettingsModel.FieldOfViewMin, UserSettingsModel.FieldOfViewMax),
					};
				}
			}
			catch (Exception ex)
			{
				Root.Dependencies.Log.Error(ex, "Failed to load user settings.");
			}
		}

		StateManager.Dispatch(new UserSettingsLoaded());
	}

	private static void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model));

		DebugStack.Add("Saved user settings.", 1);
	}
}
