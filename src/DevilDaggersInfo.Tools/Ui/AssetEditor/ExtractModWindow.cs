using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class ExtractModWindow
{
	private static string? _inputFilePath;
	private static string? _outputDirectory;
	private static bool _isExtracting;
	private static DateTime? _lastStartTime;
	private static DateTime? _lastEndTime;

	public static void Render()
	{
		if (ImGui.Begin("Extract binary"))
		{
			ImGui.SeparatorText("Input file path");
			if (ImGui.Button("Browse##file"))
			{
				NativeFileDialog.CreateOpenFileDialog(
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
				NativeFileDialog.SelectDirectory(s =>
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
					ImGui.Text(Inline.Span($"Extracted in {(_lastEndTime.Value - _lastStartTime.Value).TotalSeconds:0.000} seconds ({DateTimeUtils.FormatTimeAgo(_lastEndTime.Value)})."));
				}
			}
		}

		ImGui.End();
	}

	private static void Extract()
	{
		if (!Directory.Exists(_outputDirectory))
		{
			PopupManager.ShowError("Output directory does not exist.");
			return;
		}

		if (!File.Exists(_inputFilePath))
		{
			PopupManager.ShowError("Input file does not exist.");
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
			return ExtractionResult.Succeeded(errors);
		}
		catch (InvalidModBinaryException ex)
		{
			return ExtractionResult.Failed("Invalid mod compilation.", ex);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			return ExtractionResult.Failed("An IO error occurred.", ex);
		}
		catch (Exception ex)
		{
			return ExtractionResult.Failed("An unexpected error occurred.", ex);
		}
	}

	private static void ExtractionCompletedCallback(ExtractionResult extractionResult)
	{
		if (!extractionResult.Success)
		{
			Debug.Assert(extractionResult.Error != null, "extractionResult.Error != null");
			Debug.Assert(extractionResult.Exception != null, "extractionResult.Exception != null");
			PopupManager.ShowError($"Extraction failed: {extractionResult.Error}", extractionResult.Exception.Message);
		}

		_isExtracting = false;
		_lastEndTime = DateTime.UtcNow;

		if (extractionResult.Errors.Count > 0)
		{
			PopupManager.ShowError($"""
	            Extraction completed with {extractionResult.Errors.Count} error(s):

	            {string.Join("\n\n", extractionResult.Errors)}
	            """);
		}
	}

	// TODO: Use discriminated unions when finally added to C# (if ever).
	private sealed record ExtractionResult(bool Success, string? Error, Exception? Exception, List<string> Errors)
	{
		public static ExtractionResult Failed(string error, Exception exception)
		{
			return new(false, error, exception, []);
		}

		public static ExtractionResult Succeeded(List<string> errors)
		{
			return new(true, null, null, errors);
		}
	}
}
