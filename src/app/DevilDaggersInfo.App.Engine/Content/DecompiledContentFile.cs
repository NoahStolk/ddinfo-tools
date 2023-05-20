using DevilDaggersInfo.App.Engine.Content.Conversion;

namespace DevilDaggersInfo.App.Engine.Content;

public record DecompiledContentFile(
	IReadOnlyDictionary<string, BlobContent> Blobs,
	IReadOnlyDictionary<string, ModelContent> Models,
	IReadOnlyDictionary<string, ShaderContent> Shaders,
	IReadOnlyDictionary<string, SoundContent> Sounds,
	IReadOnlyDictionary<string, TextureContent> Textures)
{
	/// <summary>
	/// Generates the content file from <paramref name="contentRootDirectory"/> if specified, then reads the content file from <paramref name="contentFilePath"/> and returns the decompiled content.
	/// </summary>
	/// <param name="contentRootDirectory">The content root directory to generate a content file from. If the directory does not exist, or is <see langword="null" />, the file will not be generated.</param>
	/// <param name="contentFilePath">The generated content file path to read from.</param>
	/// <exception cref="InvalidOperationException">When the file at <paramref name="contentFilePath"/> does not exist.</exception>
	public static DecompiledContentFile Create(string? contentRootDirectory, string contentFilePath)
	{
		if (Directory.Exists(contentRootDirectory))
			ContentFileWriter.GenerateContentFile(contentRootDirectory, contentFilePath);

		if (!File.Exists(contentFilePath))
			throw new InvalidOperationException("The generated content file is missing. Make sure to build in DEBUG mode or copy the file generated in DEBUG mode to the RELEASE output.");

		return ContentFileReader.Read(contentFilePath);
	}
}
