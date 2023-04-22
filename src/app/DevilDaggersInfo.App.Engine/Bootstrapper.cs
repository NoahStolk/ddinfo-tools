using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Content.Conversion;

namespace DevilDaggersInfo.App.Engine;

/// <summary>
/// Used to instantiate game classes.
/// </summary>
public static class Bootstrapper
{
	/// <summary>
	/// Generates the content file from <paramref name="contentRootDirectory"/> if specified, then reads the content file from <paramref name="contentFilePath"/> and returns the decompiled content.
	/// </summary>
	/// <param name="contentRootDirectory">The content root directory to generate a content file from. If the directory does not exist, or is <see langword="null" />, the file will not be generated.</param>
	/// <param name="contentFilePath">The generated content file path required to bootstrap the game.</param>
	/// <exception cref="InvalidOperationException">When the file at <paramref name="contentFilePath"/> does not exist.</exception>
	public static DecompiledContentFile GetDecompiledContent(string? contentRootDirectory, string contentFilePath)
	{
		if (Directory.Exists(contentRootDirectory))
			ContentFileWriter.GenerateContentFile(contentRootDirectory, contentFilePath);

		if (!File.Exists(contentFilePath))
			throw new InvalidOperationException("The generated content file is missing. Make sure to build in DEBUG mode or copy the file generated in DEBUG mode to the RELEASE output.");

		return ContentFileReader.Read(contentFilePath);
	}
}
