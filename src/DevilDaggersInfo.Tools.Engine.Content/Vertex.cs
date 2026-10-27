using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Engine.Content;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Data is passed to GPU")]
public record struct Vertex(Vector3 Position, Vector2 Texture, Vector3 Normal);
