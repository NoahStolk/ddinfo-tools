namespace DevilDaggersInfo.Tools.Engine.Content;

public sealed record InternalContent(
	IReadOnlyDictionary<string, ModelContent> Models,
	IReadOnlyDictionary<string, ShaderContent> Shaders,
	IReadOnlyDictionary<string, TextureContent> Textures);
