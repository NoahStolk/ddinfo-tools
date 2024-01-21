using DevilDaggersInfo.Core.Mod.Builders;
using DevilDaggersInfo.Tools.EditorFileState;
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

				if (ImGui.Button("Compile"))
				{
					// TODO: Show progress bar and hide button when compiling.
					Task.Run(async () => await CompileAsync());
				}
			}
			else
			{
				ImGui.Text("No assets have been added to the mod.");
			}
		}

		ImGui.End();
	}

	private static async Task CompileAsync()
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

		AssetPaths mod = FileStates.Mod.Object;

		if (CreateAudio)
		{
			byte[] audioBinary = await BuildAudioBinaryAsync(mod.Audio);
			await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, $"audio{_outputFileName}"), audioBinary);
		}

		if (CreateDd)
		{
			byte[] ddBinary = await BuildDdBinaryAsync(mod.Meshes.Cast<IAssetPath>().Concat(mod.ObjectBindings).Concat(mod.Shaders).Concat(mod.Textures).ToList());
			await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, $"dd{_outputFileName}"), ddBinary);
		}
	}

	private static async Task<byte[]> BuildAudioBinaryAsync(List<AudioAssetPath> assetPaths)
	{
		AudioModBinaryBuilder builder = new();
		foreach (AudioAssetPath assetPath in assetPaths)
		{
			if (assetPath.AbsolutePath == null)
				continue;

			byte[] fileContents = await File.ReadAllBytesAsync(assetPath.AbsolutePath);
			builder.AddAudio(assetPath.AssetName, fileContents);
		}

		return builder.Compile();
	}

	private static async Task<byte[]> BuildDdBinaryAsync(List<IAssetPath> assetPaths)
	{
		DdModBinaryBuilder builder = new();
		foreach (IAssetPath assetPath in assetPaths)
		{
			switch (assetPath)
			{
				case MeshAssetPath meshAssetPath:
					if (meshAssetPath.AbsolutePath != null)
						builder.AddMesh(meshAssetPath.AssetName, await File.ReadAllBytesAsync(meshAssetPath.AbsolutePath));
					break;
				case ObjectBindingAssetPath objectBindingAssetPath:
					if (objectBindingAssetPath.AbsolutePath != null)
						builder.AddObjectBinding(objectBindingAssetPath.AssetName, await File.ReadAllBytesAsync(objectBindingAssetPath.AbsolutePath));
					break;
				case ShaderAssetPath shaderAssetPath:
					if (shaderAssetPath is { AbsoluteVertexPath: not null, AbsoluteFragmentPath: not null })
						builder.AddShader(shaderAssetPath.AssetName, await File.ReadAllBytesAsync(shaderAssetPath.AbsoluteVertexPath), await File.ReadAllBytesAsync(shaderAssetPath.AbsoluteFragmentPath));
					break;
				case TextureAssetPath textureAssetPath:
					if (textureAssetPath.AbsolutePath != null)
						builder.AddTexture(textureAssetPath.AssetName, await File.ReadAllBytesAsync(textureAssetPath.AbsolutePath));
					break;
				default:
					throw new UnreachableException();
			}
		}

		return builder.Compile();
	}
}
