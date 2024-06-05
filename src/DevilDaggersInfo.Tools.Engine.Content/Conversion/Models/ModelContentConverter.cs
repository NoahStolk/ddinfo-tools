using DevilDaggersInfo.Tools.Engine.Content.Parsers.Model;

namespace DevilDaggersInfo.Tools.Engine.Content.Conversion.Models;

internal sealed class ModelContentConverter : IContentConverter<ModelBinary>
{
	public static ModelBinary Construct(string inputPath)
	{
		ModelData modelData = ObjParser.Parse(File.ReadAllBytes(inputPath));
		return new ModelBinary(modelData.Positions, modelData.Textures, modelData.Normals, modelData.Meshes);
	}
}
