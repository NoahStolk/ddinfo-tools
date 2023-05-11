namespace DevilDaggersInfo.App.Engine.Content;

public record DecompiledContentFile(
	IReadOnlyDictionary<string, BlobContent> Blobs,
	IReadOnlyDictionary<string, ModelContent> Models,
	IReadOnlyDictionary<string, ShaderContent> Shaders,
	IReadOnlyDictionary<string, SoundContent> Sounds,
	IReadOnlyDictionary<string, TextureContent> Textures);
