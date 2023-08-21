using DevilDaggersInfo.App.Engine.Extensions;

namespace DevilDaggersInfo.App.Engine.Content.Conversion.Textures;

internal record TextureBinary(ushort Width, ushort Height, byte[] ColorData) : IBinary<TextureBinary>
{
	public ContentType ContentType => ContentType.Texture;

	private static bool IsOpaqueWhite(byte r, byte g, byte b, byte a)
		=> r == byte.MaxValue && g == byte.MaxValue && b == byte.MaxValue && a == byte.MaxValue;

	private static TextureContentType DetermineTextureContentType(IReadOnlyList<byte> colorData)
	{
		bool isWhiteOrTransparentOnly = true;
		bool isOpaque = true;
		bool isGrayScale = true;
		for (int i = 0; i < colorData.Count; i += 4)
		{
			byte r = colorData[i];
			byte g = colorData[i + 1];
			byte b = colorData[i + 2];
			byte a = colorData[i + 3];

			if (isOpaque && a < byte.MaxValue)
				isOpaque = false;

			if (isWhiteOrTransparentOnly && a != 0 && !IsOpaqueWhite(r, g, b, a))
				isWhiteOrTransparentOnly = false;

			if (isGrayScale && (r != g || r != b || g != b))
				isGrayScale = false;

			if (!isOpaque && !isWhiteOrTransparentOnly && !isGrayScale)
				break;
		}

		if (isWhiteOrTransparentOnly)
			return TextureContentType.W1;

		if (isOpaque)
			return isGrayScale ? TextureContentType.W8 : TextureContentType.Rgb24;

		return isGrayScale ? TextureContentType.Wa16 : TextureContentType.Rgba32;
	}

	public byte[] ToBytes()
	{
		TextureContentType textureContentType = DetermineTextureContentType(ColorData);

		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write((byte)textureContentType);
		bw.Write(Width);
		bw.Write(Height);

		switch (textureContentType)
		{
			case TextureContentType.W1:
				BitArray bitArray = new(ColorData.Length / 4);
				for (int i = 0; i < ColorData.Length; i += 4)
				{
					byte r = ColorData[i];
					byte g = ColorData[i + 1];
					byte b = ColorData[i + 2];
					byte a = ColorData[i + 3];
					bitArray.Set(i / 4, IsOpaqueWhite(r, g, b, a));
				}

				bw.Write(bitArray.ToBytes());
				break;
			case TextureContentType.W8:
				for (int i = 0; i < ColorData.Length; i += 4)
					bw.Write(ColorData[i]);

				break;
			case TextureContentType.Wa16:
				for (int i = 0; i < ColorData.Length; i++)
				{
					if (i % 4 is 0 or 3)
						bw.Write(ColorData[i]);
				}

				break;
			case TextureContentType.Rgb24:
				for (int i = 0; i < ColorData.Length; i++)
				{
					if (i % 4 != 3)
						bw.Write(ColorData[i]);
				}

				break;
			case TextureContentType.Rgba32:
				bw.Write(ColorData);
				break;
			default:
				throw new NotSupportedException($"{nameof(TextureContentType)} '{textureContentType}' is not supported.");
		}

		return ms.ToArray();
	}

	public static TextureBinary FromStream(BinaryReader br)
	{
		TextureContentType textureContentType = (TextureContentType)br.ReadByte();
		ushort width = br.ReadUInt16();
		ushort height = br.ReadUInt16();
		byte[] colorData = textureContentType switch
		{
			TextureContentType.W1 => ConstructTextureW1(br, width, height),
			TextureContentType.W8 => ConstructTextureW8(br, width, height),
			TextureContentType.Wa16 => ConstructTextureWa16(br, width, height),
			TextureContentType.Rgb24 => ConstructTextureRgb24(br, width, height),
			TextureContentType.Rgba32 => ConstructTextureRgba32(br, width, height),
			_ => throw new NotSupportedException($"{nameof(TextureContentType)} '{textureContentType}' is not supported."),
		};

		return new(width, height, colorData);
	}

	private static byte[] ConstructTextureW1(BinaryReader br, ushort width, ushort height)
	{
		int colorDataSize = (int)Math.Ceiling(width * height / 8f);
		byte[] colorBuffer = br.ReadBytes(colorDataSize);
		BitArray binaryColors = new(colorBuffer);

		byte[] colors = new byte[width * height * 4];
		for (int i = 0; i < colors.Length / 4; i++)
		{
			byte unifiedValue = binaryColors[i] ? (byte)0xFF : (byte)0x00;
			for (int j = 0; j < 4; j++)
				colors[i * 4 + j] = unifiedValue;
		}

		return colors;
	}

	private static byte[] ConstructTextureW8(BinaryReader br, ushort width, ushort height)
	{
		byte[] colors = new byte[width * height * 4];
		for (int i = 0; i < colors.Length; i += 4)
		{
			byte colorComponent = br.ReadByte();
			colors[i] = colorComponent;
			colors[i + 1] = colorComponent;
			colors[i + 2] = colorComponent;
			colors[i + 3] = 0xFF;
		}

		return colors;
	}

	private static byte[] ConstructTextureWa16(BinaryReader br, ushort width, ushort height)
	{
		byte[] colors = new byte[width * height * 4];
		for (int i = 0; i < colors.Length; i += 4)
		{
			byte colorComponent = br.ReadByte();
			colors[i] = colorComponent;
			colors[i + 1] = colorComponent;
			colors[i + 2] = colorComponent;
			colors[i + 3] = br.ReadByte();
		}

		return colors;
	}

	private static byte[] ConstructTextureRgb24(BinaryReader br, ushort width, ushort height)
	{
		byte[] colors = new byte[width * height * 4];
		for (int i = 0; i < colors.Length; i++)
			colors[i] = i % 4 == 3 ? (byte)0xFF : br.ReadByte();

		return colors;
	}

	private static byte[] ConstructTextureRgba32(BinaryReader br, ushort width, ushort height)
	{
		return br.ReadBytes(width * height * 4);
	}
}
