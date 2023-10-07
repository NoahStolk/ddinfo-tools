namespace DevilDaggersInfo.App.Engine.Parsers.Model;

/// <summary>
/// Represents data parsed from a model format, such as a .obj file.
/// </summary>
public record ModelData
{
	public ModelData(IReadOnlyList<Vector3> positions, IReadOnlyList<Vector2> textures, IReadOnlyList<Vector3> normals, IReadOnlyList<MeshData> meshes)
	{
		if (meshes.Count == 0)
			throw new ObjParseException("Model must have at least one mesh.");

		Positions = positions;
		Textures = textures;
		Normals = normals;
		Meshes = meshes;
	}

	public IReadOnlyList<Vector3> Positions { get; }
	public IReadOnlyList<Vector2> Textures { get; }
	public IReadOnlyList<Vector3> Normals { get; }
	public IReadOnlyList<MeshData> Meshes { get; }
}
