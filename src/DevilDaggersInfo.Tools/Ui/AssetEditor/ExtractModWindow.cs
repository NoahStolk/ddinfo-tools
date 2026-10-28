using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

internal sealed class ExtractModWindow(INativeFileDialog nativeFileDialog, PopupManager popupManager)
{
	private string? _inputFilePath;
	private string? _outputDirectory;
	private bool _isExtracting;
	private DateTime? _lastStartTime;
	private DateTime? _lastEndTime;

	public void Render()
	{
		if (ImGui.Begin("Extract binary"))
		{
			ImGui.SeparatorText("Input file path");
			if (ImGui.Button("Browse##file"))
			{
				nativeFileDialog.CreateOpenFileDialog(
					s =>
					{
						if (s != null)
							_inputFilePath = s;
					},
					null);
			}

			ImGui.SameLine();
			ImGui.Text(_inputFilePath ?? "No file selected");

			ImGui.SeparatorText("Output directory");
			if (ImGui.Button("Browse##directory"))
			{
				nativeFileDialog.SelectDirectory(s =>
				{
					if (s != null)
						_outputDirectory = s;
				});
			}

			ImGui.SameLine();
			ImGui.Text(_outputDirectory ?? "No directory selected");

			ImGui.SeparatorText("Extract");
			ImGui.BeginDisabled(_isExtracting);
			if (ImGui.Button("Extract"))
				Extract();

			ImGui.EndDisabled();

			if (_isExtracting)
			{
				ImGui.Text("Extracting...");
			}
			else
			{
				if (_lastStartTime.HasValue && _lastEndTime.HasValue)
				{
					ImGui.Text(Inline.Utf8($"Extracted in {(_lastEndTime.Value - _lastStartTime.Value).TotalSeconds:0.000} seconds ({new TimeAgo(_lastEndTime.Value)})."));
				}
			}
		}

		ImGui.End();
	}

	private void Extract()
	{
		if (!Directory.Exists(_outputDirectory))
		{
			popupManager.ShowError("Output directory does not exist.");
			return;
		}

		if (!File.Exists(_inputFilePath))
		{
			popupManager.ShowError("Input file does not exist.");
			return;
		}

		_isExtracting = true;
		_lastStartTime = DateTime.UtcNow;

		// TODO: Show progress bar.
		Task.Run(async () => ExtractionCompletedCallback(await TryExtractAsync(_inputFilePath, _outputDirectory)));
	}

	private static async Task<ExtractionResult> TryExtractAsync(string inputFilePath, string outputDirectory)
	{
		try
		{
			List<string> errors = await ExtractLogic.ExtractAsync(inputFilePath, outputDirectory);
			return new ExtractionSucceeded(errors);
		}
		catch (InvalidModBinaryException ex)
		{
			return new ExtractionFailed("Invalid mod compilation.", ex);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			return new ExtractionFailed("An IO error occurred.", ex);
		}
		catch (Exception ex)
		{
			return new ExtractionFailed("An unexpected error occurred.", ex);
		}
	}

	private void ExtractionCompletedCallback(ExtractionResult extractionResult)
	{
		switch (extractionResult)
		{
			case ExtractionFailed failed:
				popupManager.ShowError($"Extraction failed: {failed.Error}", failed.Exception.Message);
				break;

			case ExtractionSucceeded succeeded:
				if (succeeded.Errors.Count > 0)
				{
					popupManager.ShowError(
						$"""
						 Extraction completed with {succeeded.Errors.Count} error(s):

						 {string.Join("\n\n", succeeded.Errors)}
						 """);
				}

				break;
		}

		_isExtracting = false;
		_lastEndTime = DateTime.UtcNow;
	}

	private union ExtractionResult(ExtractionFailed, ExtractionSucceeded);

	private sealed record ExtractionFailed(string Error, Exception Exception);

	private sealed record ExtractionSucceeded(List<string> Errors);
}
