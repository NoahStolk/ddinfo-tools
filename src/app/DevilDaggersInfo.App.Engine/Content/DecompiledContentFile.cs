namespace DevilDaggersInfo.App.Engine.Content;

public record DecompiledContentFile(
	IReadOnlyDictionary<string, Blob> Blobs,
	IReadOnlyDictionary<string, Charset> Charsets,
	IReadOnlyDictionary<string, Model> Models,
	IReadOnlyDictionary<string, Shader> Shaders,
	IReadOnlyDictionary<string, Sound> Sounds,
	IReadOnlyDictionary<string, Texture> Textures);
