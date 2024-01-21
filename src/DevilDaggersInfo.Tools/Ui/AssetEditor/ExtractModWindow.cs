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
			if (ImGui.Button("Extract"))
			{
				// TODO: Show progress bar and hide button when extracting.
				Task.Run(async () => await ExtractAsync());
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
				catch (Exception ex) // TODO: IOException etc.
				{
					PopupManager.ShowError($"Could not write file '{fileName}' to output directory.", ex);
				}
			}
		}
	}
}
