using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.Ui.Popups;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class ExtractModWindow
{
	private static string? _inputFilePath;
	private static string? _outputDirectory;
	private static bool _isExtracting;
	private static readonly List<string> _errors = [];
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
			{
				// TODO: Show progress bar.
				Task.Run(async () => await ExtractAsync());
			}

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

	private static async Task ExtractAsync()
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
		await using FileStream fileStream = File.OpenRead(_inputFilePath);
		ModBinary modBinary = new(fileStream, ModBinaryReadFilter.AllAssets);
		foreach (ModBinaryTocEntry tocEntry in modBinary.Toc.Entries)
		{
			string subFolder = tocEntry.AssetType switch
			{
				AssetType.Audio => "Audio",
				AssetType.Mesh => "Meshes",
				AssetType.ObjectBinding => "Object Bindings",
				AssetType.Shader => "Shaders",
				AssetType.Texture => "Textures",
				_ => throw new UnreachableException($"Asset type '{tocEntry.AssetType}' not supported."),
			};

			AssetExtractionResult result = modBinary.ExtractAsset(tocEntry.Name, tocEntry.AssetType);
			foreach ((string fileName, byte[] buffer) in result.ExtractedAssetFiles)
			{
				try
				{
					string directory = Path.Combine(_outputDirectory, subFolder);
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					await File.WriteAllBytesAsync(Path.Combine(directory, fileName), buffer);
				}
				catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PathTooLongException)
				{
					_errors.Add($"Could not write file '{fileName}' to output directory.\n\n{ex.Message}");
				}
			}
		}

		_isExtracting = false;
		_lastEndTime = DateTime.UtcNow;

		if (_errors.Count > 0)
		{
			PopupManager.ShowError($"""
				Extraction completed with {_errors.Count} error(s):

				{string.Join("\n\n", _errors)}
				""");
			_errors.Clear();
		}
	}
}
