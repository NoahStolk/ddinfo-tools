using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.User.Settings.Model;
using ImGuiNET;
using Serilog.Core;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.User.Settings;

internal sealed class UserSettings(Logger logger)
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ddinfo-tools");
	private static readonly string _filePath = Path.Combine(_fileDirectory, "settings");
	private static readonly string _imguiIniFilePath = Path.Combine(_fileDirectory, "imgui.ini");

	private UserSettingsModel _model = UserSettingsModel.Default;

	public UserSettingsModel Model
	{
		get => _model;
		set
		{
			_model = value.Sanitize();
			Save();
		}
	}

	public string ModsDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "mods");

	public string DdDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "dd");

	public string ResDirectory => Path.Combine(Model.DevilDaggersInstallationDirectory, "res");

	public string ModsSurvivalPath => Path.Combine(ModsDirectory, "survival");

	public string DdSurvivalPath => Path.Combine(DdDirectory, "survival");

	public string ResAudioPath => Path.Combine(ResDirectory, "audio");

	public string ResDdPath => Path.Combine(ResDirectory, "dd");

	public void Load()
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
			logger.Error(ex, "Failed to load user settings.");
		}
	}

	private void Save()
	{
		Directory.CreateDirectory(_fileDirectory);
		File.WriteAllText(_filePath, JsonSerializer.Serialize(_model, UserJsonModelsContext.Default.UserSettingsModel));
	}

	public void LoadImGuiIni()
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
			logger.Error(ex, "Could not load imgui.ini.");
		}

		logger.Debug("Loaded imgui.ini");
	}

	public void SaveImGuiIni(ImGuiIOPtr io)
	{
		Directory.CreateDirectory(_fileDirectory);

		string iniContents = ImGui.SaveIniSettingsToMemory(out _);
		File.WriteAllText(_imguiIniFilePath, iniContents);

		logger.Debug("Saved imgui.ini");

		io.WantSaveIniSettings = false;
	}
}
