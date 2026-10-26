namespace DevilDaggersInfo.Tools.Engine.Content;

public sealed class ModelContent
{
	public ModelContent(Dictionary<MeshContent, TextureContent> meshes)
	{
		if (meshes.Count == 0)
			throw new InvalidOperationException("Model must have meshes.");

		MainMesh = meshes.Keys.First();
	}

	public MeshContent MainMesh { get; }
}
