namespace DevilDaggersInfo.App.Engine.Content.Conversion;

internal enum TextureContentType : byte
{
	/// <summary>
	/// Texture where every pixel is stored in 1 bit and therefore only supports 2 colors (white and transparent) -- typically used for fonts.
	/// </summary>
	W1 = 0x00,

	/// <summary>
	/// Texture where every pixel consists of 8-bit RGB color components. Alpha is not stored and should be set to 255.
	/// </summary>
	Rgb24 = 0x01,

	/// <summary>
	/// Texture where every pixel consists of 8-bit RGBA color components.
	/// </summary>
	Rgba32 = 0x02,

	/// <summary>
	/// Texture where every pixel consists of a single 8-bit color component used for R, G, and B channels (grayscale). Alpha is not stored and should be set to 255.
	/// </summary>
	W8 = 0x03,

	/// <summary>
	/// Texture where every pixel consists of a single 8-bit color component used for R, G, and B channels (grayscale), and an 8-bit color component used for the alpha channel.
	/// </summary>
	Wa16 = 0x04,
}
