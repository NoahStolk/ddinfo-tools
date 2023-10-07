namespace DevilDaggersInfo.App.Engine.Parsers.Sound;

public static class WaveParser
{
	private const int _fmtMinimumSize = 16;
	private const int _audioFormat = 1;

	private static ReadOnlySpan<byte> RiffHeader => "RIFF"u8;
	private static ReadOnlySpan<byte> FormatHeader => "WAVE"u8;
	private static ReadOnlySpan<byte> JunkHeader => "JUNK"u8;
	private static ReadOnlySpan<byte> FmtHeader => "fmt "u8;
	private static ReadOnlySpan<byte> DataHeader => "data"u8;

	public static SoundData Parse(byte[] fileContents)
	{
		using MemoryStream ms = new(fileContents);
		using BinaryReader br = new(ms);
		ReadOnlySpan<byte> riffHeader = br.ReadBytes(4);
		if (!riffHeader.SequenceEqual(RiffHeader))
			throw new WaveParseException($"Expected {FormatBytes(RiffHeader)} header (got {FormatBytes(riffHeader)}).");

		_ = br.ReadInt32(); // Amount of bytes remaining at this point (after these 4).

		ReadOnlySpan<byte> format = br.ReadBytes(4);
		if (!format.SequenceEqual(FormatHeader))
			throw new WaveParseException($"Expected {FormatBytes(FormatHeader)} header (got {FormatBytes(format)}).");

		ReadOnlySpan<byte> fmtOrJunkHeader = br.ReadBytes(4);
		if (fmtOrJunkHeader.SequenceEqual(JunkHeader))
		{
			int junkSize = br.ReadInt32();
			br.BaseStream.Seek(junkSize, SeekOrigin.Current);

			fmtOrJunkHeader = br.ReadBytes(4);
		}

		if (!fmtOrJunkHeader.SequenceEqual(FmtHeader))
			throw new WaveParseException($"Expected {FormatBytes(FmtHeader)} header (got {FormatBytes(fmtOrJunkHeader)}).");

		int fmtSize = br.ReadInt32();
		if (fmtSize < _fmtMinimumSize)
			throw new WaveParseException($"Expected FMT data chunk size to be at least {_fmtMinimumSize} (got {fmtSize}).");

		short audioFormat = br.ReadInt16();
		if (audioFormat != _audioFormat)
			throw new WaveParseException($"Expected audio format to be {_audioFormat} (got {audioFormat}).");

		short channels = br.ReadInt16();
		int sampleRate = br.ReadInt32();
		int byteRate = br.ReadInt32();
		short blockAlign = br.ReadInt16();
		short bitsPerSample = br.ReadInt16();

		br.BaseStream.Seek(_fmtMinimumSize - fmtSize, SeekOrigin.Current);

		int expectedByteRate = sampleRate * channels * bitsPerSample / 8;
		int expectedBlockAlign = channels * bitsPerSample / 8;
		if (byteRate != expectedByteRate)
			throw new WaveParseException($"Expected byte rate to be {expectedByteRate} (got {byteRate}).");
		if (blockAlign != expectedBlockAlign)
			throw new WaveParseException($"Expected block align to be {expectedBlockAlign} (got {blockAlign}).");

		for (long i = br.BaseStream.Position; i < br.BaseStream.Length - (DataHeader.Length + sizeof(int)); i += 4)
		{
			ReadOnlySpan<byte> dataHeader = br.ReadBytes(4);
			if (!dataHeader.SequenceEqual(DataHeader))
				continue;

			int dataSize = br.ReadInt32();
			byte[] data = br.ReadBytes(dataSize);

			int sampleCount = dataSize / (bitsPerSample / 8) / channels;
			double lengthInSeconds = sampleCount / (double)sampleRate;
			return new(channels, sampleRate, byteRate, blockAlign, bitsPerSample, data, sampleCount, lengthInSeconds);
		}

		throw new WaveParseException($"Could not find {FormatBytes(DataHeader)} header.");

		static string FormatBytes(ReadOnlySpan<byte> bytes)
		{
			Span<char> hexChars = stackalloc char[bytes.Length * 2];
			Span<char> asciiChars = stackalloc char[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				string hex = bytes[i].ToString("X2");
				hexChars[i * 2] = hex[0];
				hexChars[i * 2 + 1] = hex[1];

				asciiChars[i] = (char)bytes[i];
			}

			return $"{hexChars} ('{asciiChars}')";
		}
	}
}
