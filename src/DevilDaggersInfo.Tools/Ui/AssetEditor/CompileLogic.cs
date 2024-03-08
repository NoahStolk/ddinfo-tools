using DevilDaggersInfo.Core.Mod.Builders;
using DevilDaggersInfo.Core.Mod.Exceptions;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using DevilDaggersInfo.Tools.Ui.Popups;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class CompileLogic
{
	public static async Task CompileAsync(string outputDirectory, string outputFileName, bool createAudio, bool createDd)
	{
		if (!createAudio && !createDd)
			return;

		AssetPaths mod = FileStates.Mod.Object;

		if (createAudio)
			await CompileAudioAsync(mod, outputDirectory, outputFileName);

		if (createDd)
			await CompileDdAsync(mod, outputDirectory, outputFileName);
	}

	private static async Task CompileAudioAsync(AssetPaths mod, string outputDirectory, string outputFileName)
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
			await File.WriteAllBytesAsync(Path.Combine(outputDirectory, $"audio{outputFileName}"), audioBinary);
		}
		catch (Exception ex) when (ex.IsFileIoException())
		{
			PopupManager.ShowError("Could not write audio binary to output directory.", ex);
		}
	}

	private static async Task CompileDdAsync(AssetPaths mod, string outputDirectory, string outputFileName)
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
			await File.WriteAllBytesAsync(Path.Combine(outputDirectory, $"dd{outputFileName}"), ddBinary);
		}
		catch (Exception ex) when (ex.IsFileIoException())
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
					throw new UnreachableException($"Unknown asset type '{assetPath.GetType().Name}'.");
			}
		}

		return builder.Compile();
	}
}
