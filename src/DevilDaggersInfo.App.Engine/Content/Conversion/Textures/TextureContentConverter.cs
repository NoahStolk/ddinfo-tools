using DevilDaggersInfo.App.Engine.Parsers.Texture;

namespace DevilDaggersInfo.App.Engine.Content.Conversion.Textures;

internal sealed class TextureContentConverter : IContentConverter<TextureBinary>
{
	public static TextureBinary Construct(string inputPath)
	{
		TextureData textureData = TgaParser.Parse(File.ReadAllBytes(inputPath));
		return new(textureData.Width, textureData.Height, textureData.ColorData);
	}
}
