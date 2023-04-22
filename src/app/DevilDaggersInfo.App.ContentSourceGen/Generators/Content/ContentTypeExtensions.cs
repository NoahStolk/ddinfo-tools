using DevilDaggersInfo.App.ContentSourceGen.Utils;

namespace DevilDaggersInfo.App.ContentSourceGen.Generators.Content;

public static class ContentTypeExtensions
{
	public static ContentType GetContentTypeFromFileExtension(this string fileExtension)
	{
		return fileExtension switch
		{
			".vert" => ContentType.Shader,
			".geom" => ContentType.Shader,
			".frag" => ContentType.Shader,
			".bin" => ContentType.Blob,
			".txt" => ContentType.Charset,
			".map" => ContentType.Map,
			".obj" => ContentType.Model,
			".wav" => ContentType.Sound,
			".tga" => ContentType.Texture,
			_ => throw new InvalidOperationException($"Unknown content file extension: {fileExtension}"),
		};
	}

	public static string GetClassName(this ContentType contentType)
	{
		return contentType switch
		{
			ContentType.Shader => "Shaders",
			ContentType.Blob => "Blobs",
			ContentType.Charset => "Charsets",
			ContentType.Map => "Maps",
			ContentType.Model => "Models",
			ContentType.Sound => "Sounds",
			ContentType.Texture => "Textures",
			_ => throw new InvalidOperationException($"Unknown content type: {contentType}"),
		};
	}

	public static string GetContentTypeName(this ContentType contentType)
	{
		return contentType switch
		{
			ContentType.Shader => $"{Constants.RootNamespace}.Content.Shader",
			ContentType.Blob => $"{Constants.RootNamespace}.Content.Blob",
			ContentType.Charset => $"{Constants.RootNamespace}.Content.Charset",
			ContentType.Map => $"{Constants.RootNamespace}.Content.Map",
			ContentType.Model => $"{Constants.RootNamespace}.Content.Model",
			ContentType.Sound => $"{Constants.RootNamespace}.Content.Sound",
			ContentType.Texture => $"{Constants.RootNamespace}.Content.Texture",
			_ => throw new InvalidOperationException($"Unknown content type: {contentType}"),
		};
	}
}
