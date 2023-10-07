namespace DevilDaggersInfo.App.Engine.Parsers.Texture;

public static class TgaParser
{
	public static TextureData Parse(byte[] fileContents)
	{
		using MemoryStream ms = new(fileContents);
		using BinaryReader br = new(ms);

		// Header
		byte idLength = br.ReadByte();
		byte colorMapType = br.ReadByte();
		if (colorMapType != 0)
			throw new TgaParseException($"TGA with color map type {colorMapType} is not supported.");

		byte imageType = br.ReadByte();
		if (imageType != 2)
			throw new TgaParseException($"TGA with image type {imageType} is not supported.");

		// Color map spec
		_ = br.ReadUInt16(); // Index of first entry
		ushort colorMapLength = br.ReadUInt16();
		_ = br.ReadByte(); // Entry size

		// Image spec
		_ = br.ReadUInt16(); // Origin X
		_ = br.ReadUInt16(); // Origin Y
		ushort width = br.ReadUInt16();
		ushort height = br.ReadUInt16();
		byte pixelDepth = br.ReadByte();
		byte imageDescriptor = br.ReadByte();
		bool rightToLeft = BitUtils.IsBitSet(imageDescriptor, 4);
		bool topToBottom = BitUtils.IsBitSet(imageDescriptor, 5);

		// Skip image ID and color map.
		br.BaseStream.Seek(idLength, SeekOrigin.Current);
		br.BaseStream.Seek(colorMapLength, SeekOrigin.Current);

		// Image data
		byte[] colorData = pixelDepth switch
		{
			32 => ReadRgba(br, width, height, rightToLeft, topToBottom),
			24 => ReadRgb(br, width, height, rightToLeft, topToBottom),
			_ => throw new TgaParseException($"TGA with pixel depth {pixelDepth} is not supported."),
		};

		return new(width, height, colorData);
	}

	private static byte[] ReadRgba(BinaryReader br, ushort width, ushort height, bool rightToLeft, bool topToBottom)
	{
		byte[] encodedPixels = br.ReadBytes(width * height * 4);

		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		for (int i = topToBottom ? 0 : height - 1; topToBottom ? i < height : i >= 0; i += topToBottom ? 1 : -1)
		{
			for (int j = rightToLeft ? width - 1 : 0; rightToLeft ? j >= 0 : j < width; j += rightToLeft ? -1 : 1)
			{
				int pixelIndex = (i * width + j) * 4;
				bw.Write(encodedPixels[pixelIndex + 2]); // R
				bw.Write(encodedPixels[pixelIndex + 1]); // G
				bw.Write(encodedPixels[pixelIndex + 0]); // B
				bw.Write(encodedPixels[pixelIndex + 3]); // A
			}
		}

		return ms.ToArray();
	}

	private static byte[] ReadRgb(BinaryReader br, ushort width, ushort height, bool rightToLeft, bool topToBottom)
	{
		byte[] encodedPixels = br.ReadBytes(width * height * 3);

		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		for (int i = topToBottom ? 0 : height - 1; topToBottom ? i < height : i >= 0; i += topToBottom ? 1 : -1)
		{
			for (int j = rightToLeft ? width - 1 : 0; rightToLeft ? j >= 0 : j < width; j += rightToLeft ? -1 : 1)
			{
				int pixelIndex = (i * width + j) * 3;
				bw.Write(encodedPixels[pixelIndex + 2]); // R
				bw.Write(encodedPixels[pixelIndex + 1]); // G
				bw.Write(encodedPixels[pixelIndex + 0]); // B
				bw.Write((byte)0xFF);
			}
		}

		return ms.ToArray();
	}
}
