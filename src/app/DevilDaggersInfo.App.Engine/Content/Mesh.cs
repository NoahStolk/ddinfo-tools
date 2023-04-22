namespace DevilDaggersInfo.App.Engine.Content;

public class Mesh
{
	public Mesh(Vertex[] vertices, uint[] indices)
	{
		Vertices = vertices;
		Indices = indices;
	}

	public Vertex[] Vertices { get; }
	public uint[] Indices { get; }
}
