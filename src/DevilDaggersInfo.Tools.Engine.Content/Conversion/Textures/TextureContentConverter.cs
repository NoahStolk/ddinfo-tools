using DevilDaggersInfo.Tools.Engine.Content.Parsers.Texture;

namespace DevilDaggersInfo.Tools.Engine.Content.Conversion.Textures;

internal sealed class TextureContentConverter : IContentConverter<TextureBinary>
{
	public static TextureBinary Construct(string inputPath)
	{
		TextureData textureData = TgaParser.Parse(File.ReadAllBytes(inputPath));
		return new TextureBinary(textureData.Width, textureData.Height, textureData.ColorData);
	}
}
