using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Tools.Extensions;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class ExtractLogic
{
	public static async Task<List<string>> ExtractAsync(string inputFilePath, string outputDirectory)
	{
		List<string> errors = [];

		await using FileStream fileStream = File.OpenRead(inputFilePath);
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
					string directory = Path.Combine(outputDirectory, subFolder);
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					await File.WriteAllBytesAsync(Path.Combine(directory, fileName), buffer);
				}
				catch (Exception ex) when (ex.IsFileIoException())
				{
					errors.Add($"Could not write file '{fileName}' to output directory.\n\n{ex.Message}");
				}
			}
		}

		return errors;
	}
}
