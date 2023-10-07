namespace DevilDaggersInfo.App.Engine.Content;

public class ModelContent
{
	public ModelContent(Dictionary<MeshContent, TextureContent> meshes)
	{
		if (meshes.Count == 0)
			throw new InvalidOperationException("Model must have meshes.");

		Meshes = meshes;
		MainMesh = meshes.Keys.First();
	}

	public Dictionary<MeshContent, TextureContent> Meshes { get; }

	public MeshContent MainMesh { get; }
}
