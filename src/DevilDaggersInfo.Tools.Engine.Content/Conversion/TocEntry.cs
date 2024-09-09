namespace DevilDaggersInfo.Tools.Engine.Content.Conversion;

/// <summary>
/// Represents an entry in the table of contents (TOC) buffer in the content file.
/// </summary>
internal sealed class TocEntry
{
	public TocEntry(ContentType contentType, string name, uint length)
	{
		ContentType = contentType;
		Name = name;
		Length = length;
	}

	public ContentType ContentType { get; }

	public string Name { get; }

	public uint Length { get; }

	public override string ToString()
	{
		return $"{ContentType}: \"{Name}\" ({Length} bytes)";
	}
}
