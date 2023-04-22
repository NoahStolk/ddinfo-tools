namespace DevilDaggersInfo.App.Engine.Content;

public class Model
{
	public Model(Dictionary<Mesh, Texture> meshes)
	{
		if (meshes.Count == 0)
			throw new InvalidOperationException("Model must have meshes.");

		Meshes = meshes;
		MainMesh = meshes.Keys.First();
	}

	public Dictionary<Mesh, Texture> Meshes { get; }

	public Mesh MainMesh { get; }
}
