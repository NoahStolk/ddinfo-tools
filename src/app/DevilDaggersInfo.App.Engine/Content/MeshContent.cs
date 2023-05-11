namespace DevilDaggersInfo.App.Engine.Content;

public class MeshContent
{
	public MeshContent(Vertex[] vertices, uint[] indices)
	{
		Vertices = vertices;
		Indices = indices;
	}

	public Vertex[] Vertices { get; }
	public uint[] Indices { get; }
}
