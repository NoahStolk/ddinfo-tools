using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;

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
				if (_lastStartTime != null && _lastEndTime != null)
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
		Task.Run(async () => ExtractionCompletedCallback(await ExtractLogic.ExtractAsync(_inputFilePath, _outputDirectory)));
	}

	private static void ExtractionCompletedCallback(List<string> errors)
	{
		_isExtracting = false;
		_lastEndTime = DateTime.UtcNow;

		if (errors.Count > 0)
		{
			PopupManager.ShowError($"""
	            Extraction completed with {errors.Count} error(s):

	            {string.Join("\n\n", errors)}
	            """);
		}
	}
}
