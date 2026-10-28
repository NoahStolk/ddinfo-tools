using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

internal sealed class CompileModWindow(INativeFileDialog nativeFileDialog, PopupManager popupManager, FileStates fileStates, UserSettings userSettings)
{
	private string _outputDirectory = userSettings.ModsDirectory;
	private string _outputFileName = "mod";
	private bool _isCompiling;
	private DateTime? _lastStartTime;
	private DateTime? _lastEndTime;

	private bool CreateAudio => fileStates.Mod.Object.Audio.Count > 0;
	private bool CreateDd =>
		fileStates.Mod.Object.Meshes.Count > 0 ||
		fileStates.Mod.Object.ObjectBindings.Count > 0 ||
		fileStates.Mod.Object.Shaders.Count > 0 ||
		fileStates.Mod.Object.Textures.Count > 0;

	public void Render()
	{
		if (ImGui.Begin("Compile mod"))
		{
			ImGui.SeparatorText("File locations");
			if (ImGui.Button("Browse"))
			{
				nativeFileDialog.SelectDirectory(s =>
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
					ImGui.BulletText(Inline.Utf8($"audio{_outputFileName}"));
				if (CreateDd)
					ImGui.BulletText(Inline.Utf8($"dd{_outputFileName}"));

				ImGui.BeginDisabled(_isCompiling);
				if (ImGui.Button("Compile"))
					Compile();

				ImGui.EndDisabled();

				if (_isCompiling)
				{
					ImGui.Text("Compiling...");
				}
				else
				{
					if (_lastStartTime.HasValue && _lastEndTime.HasValue)
					{
						ImGui.Text(Inline.Utf8($"Compiled in {(_lastEndTime.Value - _lastStartTime.Value).TotalSeconds:0.000} seconds ({new TimeAgo(_lastEndTime.Value)})."));
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

	private void Compile()
	{
		if (!Directory.Exists(_outputDirectory))
		{
			popupManager.ShowError("Output directory does not exist.");
			return;
		}

		if (_outputFileName.Length == 0)
		{
			popupManager.ShowError("File name cannot be empty.");
			return;
		}

		if (_outputFileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
		{
			popupManager.ShowError("File name contains invalid characters.");
			return;
		}

		// TODO: Ask to overwrite the file if it already exists instead.
		if (File.Exists(Path.Combine(_outputDirectory, $"audio{_outputFileName}")))
		{
			popupManager.ShowError($"File 'audio{_outputFileName}' already exists in the output directory.");
			return;
		}

		if (File.Exists(Path.Combine(_outputDirectory, $"dd{_outputFileName}")))
		{
			popupManager.ShowError($"File 'dd{_outputFileName}' already exists in the output directory.");
			return;
		}

		_isCompiling = true;
		_lastStartTime = DateTime.UtcNow;

		// TODO: Show progress bar.
		Task.Run(async () => CompilationCompletedCallback(await TryCompileAsync()));
	}

	private async Task<CompilationResult> TryCompileAsync()
	{
		try
		{
			await CompileLogic.CompileAsync(fileStates.Mod.Object, _outputDirectory, _outputFileName, CreateAudio, CreateDd);
			return new CompilationSucceeded();
		}
		catch (InvalidObjException ex)
		{
			return new CompilationFailed("Invalid OBJ file.", ex);
		}
		catch (InvalidModCompilationException ex)
		{
			return new CompilationFailed("Invalid mod compilation.", ex);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			return new CompilationFailed("An IO error occurred.", ex);
		}
		catch (Exception ex)
		{
			return new CompilationFailed("An unexpected error occurred.", ex);
		}
	}

	private void CompilationCompletedCallback(CompilationResult compilationResult)
	{
		switch (compilationResult)
		{
			case CompilationFailed failed: popupManager.ShowError($"Compilation failed: {failed.Error}", failed.Exception.Message); break;
			case CompilationSucceeded _: break;
		}

		_isCompiling = false;
		_lastEndTime = DateTime.UtcNow;
	}

	private union CompilationResult(CompilationFailed, CompilationSucceeded);

	private sealed record CompilationFailed(string Error, Exception Exception);

	private sealed record CompilationSucceeded;
}
