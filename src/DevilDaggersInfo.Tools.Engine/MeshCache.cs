using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

/// <summary>
/// Owns one <see cref="GpuMesh"/> per <see cref="MeshContent"/> and uploads on first use.
/// </summary>
/// <remarks>
/// <para>
/// Meshes are keyed on reference identity, not value. Uploading lazily is what keeps a mesh internally consistent: a
/// <see cref="GpuMesh"/> always takes its vertices and its indices from the same <see cref="MeshContent"/> instance, so
/// the two can never come from different generations of the game content.
/// </para>
/// <para>
/// This deliberately does not implement <see cref="IDisposable"/>. Its contents may only be released while a GL context
/// is current, which is not true at container teardown, so <see cref="Clear"/> is called explicitly when the game
/// content is reloaded. At process exit the driver reclaims everything anyway.
/// </para>
/// </remarks>
public sealed class MeshCache(GL gl)
{
	// ReferenceEqualityComparer is passed explicitly so that giving MeshContent value equality later cannot silently turn
	// this into a structural comparison over two arrays.
	private readonly Dictionary<MeshContent, GpuMesh> _meshes = new(ReferenceEqualityComparer.Instance);

	public GpuMesh GetOrCreate(MeshContent content)
	{
		if (_meshes.TryGetValue(content, out GpuMesh? mesh))
			return mesh;

		mesh = GpuMesh.Create(gl, content);
		_meshes.Add(content, mesh);
		return mesh;
	}

	/// <summary>
	/// Disposes every cached mesh. Callers holding <see cref="GpuMesh"/> references must drop them first.
	/// </summary>
	public void Clear()
	{
		foreach (GpuMesh mesh in _meshes.Values)
			mesh.Dispose();

		_meshes.Clear();
	}
}
