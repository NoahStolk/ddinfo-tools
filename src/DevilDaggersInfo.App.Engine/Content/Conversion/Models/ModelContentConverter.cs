using DevilDaggersInfo.App.Engine.Parsers.Model;

namespace DevilDaggersInfo.App.Engine.Content.Conversion.Models;

internal sealed class ModelContentConverter : IContentConverter<ModelBinary>
{
	public static ModelBinary Construct(string inputPath)
	{
		ModelData modelData = ObjParser.Parse(File.ReadAllBytes(inputPath));
		return new(modelData.Positions, modelData.Textures, modelData.Normals, modelData.Meshes);
	}
}
