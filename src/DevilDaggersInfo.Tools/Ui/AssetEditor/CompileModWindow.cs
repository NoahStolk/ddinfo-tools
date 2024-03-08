using DevilDaggersInfo.Core.Mod.Builders;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class CompileModWindow
{
	private static string _outputDirectory = UserSettings.ModsDirectory;
	private static string _outputFileName = "mod";
	private static bool _isCompiling;
	private static DateTime? _lastStartTime;
	private static DateTime? _lastEndTime;

	private static bool CreateAudio => FileStates.Mod.Object.Audio.Count > 0;
	private static bool CreateDd => FileStates.Mod.Object.Meshes.Count > 0 || FileStates.Mod.Object.ObjectBindings.Count > 0 || FileStates.Mod.Object.Shaders.Count > 0 || FileStates.Mod.Object.Textures.Count > 0;

	public static void Render()
	{
		if (ImGui.Begin("Compile mod"))
		{
			ImGui.SeparatorText("File locations");
			if (ImGui.Button("Browse"))
			{
				NativeFileDialog.SelectDirectory(s =>
				{
					if (s != null)
						_outputDirectory = s;
				});
			}

			ImGui.SameLine();
			ImGui.Text(_outputDirectory);

			ImGui.InputText("Output file name", ref _outputFileName, 64);

			ImGui.SeparatorText("Compile");
			if (CreateAudio || CreateDd)
			{
				ImGui.Text("The following files will be created:");
				if (CreateAudio)
					ImGui.BulletText(Inline.Span($"audio{_outputFileName}"));
				if (CreateDd)
					ImGui.BulletText(Inline.Span($"dd{_outputFileName}"));

				ImGui.BeginDisabled(_isCompiling);
				if (ImGui.Button("Compile"))
				{
					if (!Directory.Exists(_outputDirectory))
					{
						PopupManager.ShowError("Output directory does not exist.");
						return;
					}

					if (_outputFileName.Length == 0)
					{
						PopupManager.ShowError("File name cannot be empty.");
						return;
					}

					if (_outputFileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
					{
						PopupManager.ShowError("File name contains invalid characters.");
						return;
					}

					if (File.Exists(Path.Combine(_outputDirectory, $"audio{_outputFileName}")))
					{
						PopupManager.ShowError($"File 'audio{_outputFileName}' already exists in the output directory.");
						return;
					}

					if (File.Exists(Path.Combine(_outputDirectory, $"dd{_outputFileName}")))
					{
						PopupManager.ShowError($"File 'dd{_outputFileName}' already exists in the output directory.");
						return;
					}

					_isCompiling = true;
					_lastStartTime = DateTime.UtcNow;

					// TODO: Show progress bar.
					Task.Run(async () => await CompileLogic.CompileAsync(_outputDirectory, _outputFileName, CreateAudio, CreateDd));

					_isCompiling = false;
					_lastEndTime = DateTime.UtcNow;
				}

				ImGui.EndDisabled();

				if (_isCompiling)
				{
					ImGui.Text("Compiling...");
				}
				else
				{
					if (_lastStartTime != null && _lastEndTime != null)
					{
						ImGui.Text(Inline.Span($"Compiled in {(_lastEndTime.Value - _lastStartTime.Value).TotalSeconds:0.000} seconds ({DateTimeUtils.FormatTimeAgo(_lastEndTime.Value)})."));
					}
				}
			}
			else
			{
				ImGui.Text("No assets have been added to the mod.");
			}
		}

		ImGui.End();
	}
}
