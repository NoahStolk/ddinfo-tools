using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.User.Settings.Model;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.User.Settings;

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
			_model = value.Sanitize();
			Save();
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
		if (!File.Exists(_filePath))
			return;

		try
		{
			string json = File.ReadAllText(_filePath);

			UserSettingsModel? deserializedModel = JsonSerializer.Deserialize(json, UserJsonModelsContext.Default.UserSettingsModel);
			if (deserializedModel != null)
				_model = deserializedModel.Sanitize();
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, "Failed to load user settings.");
		}
	}

	private static void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model, UserJsonModelsContext.Default.UserSettingsModel));
	}
}
