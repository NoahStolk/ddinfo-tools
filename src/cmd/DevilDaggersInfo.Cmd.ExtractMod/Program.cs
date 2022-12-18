using DevilDaggersInfo.Core.Mod;

string inputPath = args[0];
string outputDirectory = args[1];

byte[] fileContents = File.ReadAllBytes(inputPath);
ModBinary modBinary = new(fileContents, ModBinaryReadFilter.AllAssets);
foreach (ModBinaryChunk chunk in modBinary.Toc.Chunks)
{
	// TODO: Write to directory.
	modBinary.ExtractAsset(chunk.Name, chunk.AssetType);
}
