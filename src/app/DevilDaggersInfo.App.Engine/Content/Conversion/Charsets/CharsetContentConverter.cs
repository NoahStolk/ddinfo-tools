namespace DevilDaggersInfo.App.Engine.Content.Conversion.Charsets;

internal sealed class CharsetContentConverter : IContentConverter<CharsetBinary>
{
	public static CharsetBinary Construct(string inputPath)
	{
		byte[] characters = File.ReadAllBytes(inputPath);
		return new(characters);
	}
}
