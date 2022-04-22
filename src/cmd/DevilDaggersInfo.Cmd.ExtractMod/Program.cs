using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Enums;

string inputPath = args[0];
string outputDirectory = args[1];

byte[] fileContents = File.ReadAllBytes(inputPath);
ModBinary modBinary = new(fileContents, ModBinaryReadComprehensiveness.All);
modBinary.ExtractAssets(outputDirectory);
