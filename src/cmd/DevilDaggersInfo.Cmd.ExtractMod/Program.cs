using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Enums;

string inputPath = args[0];
string outputDirectory = args[1];

byte[] fileContents = File.ReadAllBytes(inputPath);
ModBinary modBinary = new(fileContents, ModBinaryReadComprehensiveness.All);
foreach (ModBinaryChunk chunk in modBinary.Chunks)
{
	// TODO: Write to directory.
	modBinary.ExtractAsset(chunk.Name, chunk.AssetType);
}
