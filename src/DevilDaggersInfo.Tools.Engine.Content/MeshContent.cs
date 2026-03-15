namespace DevilDaggersInfo.Tools.Engine.Content;

public sealed class MeshContent(Vertex[] vertices, uint[] indices)
{
	public Vertex[] Vertices { get; } = vertices;
	public uint[] Indices { get; } = indices;
}
