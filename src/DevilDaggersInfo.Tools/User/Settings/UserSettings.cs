using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.User.Settings.Model;
using ImGuiNET;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.User.Settings;

public static class UserSettings
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ddinfo-tools");
	private static readonly string _filePath = Path.Combine(_fileDirectory, "settings");
	private static readonly string _imguiIniFilePath = Path.Combine(_fileDirectory, "imgui.ini");

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
		catch (Exception ex) when (ex is JsonException || ex.IsFileIoException())
		{
			Root.Log.Error(ex, "Failed to load user settings.");
		}
	}

	private static void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model, UserJsonModelsContext.Default.UserSettingsModel));
	}

	public static void LoadImGuiIni()
	{
		if (!File.Exists(_filePath))
			return;

		try
		{
			string iniContents = File.ReadAllText(_imguiIniFilePath);
			ImGui.LoadIniSettingsFromMemory(iniContents);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			Root.Log.Error(ex, "Could not load imgui.ini.");
		}

		DebugWindow.Add("Loaded imgui.ini");
	}

	public static void SaveImGuiIni()
	{
		Directory.CreateDirectory(_fileDirectory);

		string iniContents = ImGui.SaveIniSettingsToMemory(out _);
		File.WriteAllText(_imguiIniFilePath, iniContents);

		DebugWindow.Add("Saved imgui.ini");
	}
}
