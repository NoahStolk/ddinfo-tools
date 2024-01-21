using DevilDaggersInfo.Core.Mod.Builders;
using DevilDaggersInfo.Core.Mod.Exceptions;
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
					// TODO: Show progress bar.
					Task.Run(async () => await CompileAsync());
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

		_isCompiling = true;
		_lastStartTime = DateTime.UtcNow;
		AssetPaths mod = FileStates.Mod.Object;

		if (CreateAudio)
			await CompileAudioAsync(mod);

		if (CreateDd)
			await CompileDdAsync(mod);

		_isCompiling = false;
		_lastEndTime = DateTime.UtcNow;
	}

	private static async Task CompileAudioAsync(AssetPaths mod)
	{
		byte[]? audioBinary;
		try
		{
			audioBinary = await BuildAudioBinaryAsync(mod.Audio);
		}
		catch (Exception ex) when (ex is InvalidModCompilationException or IOException)
		{
			PopupManager.ShowError("Could not compile audio binary.", ex);
			return;
		}

		try
		{
			await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, $"audio{_outputFileName}"), audioBinary);
		}
		catch (Exception ex) when (ex is IOException) // TODO: Catch more specific exceptions.
		{
			PopupManager.ShowError("Could not write audio binary to output directory.", ex);
		}
	}

	private static async Task CompileDdAsync(AssetPaths mod)
	{
		byte[]? ddBinary;
		try
		{
			ddBinary = await BuildDdBinaryAsync(mod.Meshes.Cast<IAssetPath>().Concat(mod.ObjectBindings).Concat(mod.Shaders).Concat(mod.Textures).ToList());
		}
		catch (Exception ex) when (ex is InvalidModCompilationException or IOException)
		{
			PopupManager.ShowError("Could not compile dd binary.", ex);
			return;
		}

		try
		{
			await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, $"dd{_outputFileName}"), ddBinary);
		}
		catch (Exception ex) when (ex is IOException) // TODO: Catch more specific exceptions.
		{
			PopupManager.ShowError("Could not write dd binary to output directory.", ex);
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
			builder.AddAudio(assetPath.AssetName, fileContents, assetPath.Loudness);
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
