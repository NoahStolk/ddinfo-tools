namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Model;

public sealed record MeshData(string MaterialName, IReadOnlyList<Face> Faces);
