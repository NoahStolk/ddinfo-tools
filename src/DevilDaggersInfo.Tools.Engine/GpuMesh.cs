using DevilDaggersInfo.Tools.Engine.Content;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Engine;

/// <summary>
/// Owns the GPU-side buffers backing a <see cref="MeshContent"/>: a vertex array object, its vertex buffer, and its
/// element buffer. All three are kept alive for the lifetime of this instance and deleted together on <see cref="Dispose"/>.
/// </summary>
public sealed class GpuMesh : IDisposable
{
	private readonly GL _gl;
	private readonly uint _vao;
	private readonly uint _vbo;
	private readonly uint _ebo;
	private readonly uint _indexCount;

	private bool _disposed;

	private GpuMesh(GL gl, uint vao, uint vbo, uint ebo, uint indexCount)
	{
		_gl = gl;
		_vao = vao;
		_vbo = vbo;
		_ebo = ebo;
		_indexCount = indexCount;
	}

	public static unsafe GpuMesh Create(GL gl, MeshContent mesh)
	{
		if (mesh.Vertices.Length == 0)
			throw new ArgumentException("Mesh has no vertices.", nameof(mesh));

		if (mesh.Indices.Length == 0)
			throw new ArgumentException("Mesh has no indices.", nameof(mesh));

		uint vao = gl.GenVertexArray();
		gl.BindVertexArray(vao);

		// The element array buffer binding is part of VAO state, so it must be bound while the VAO is bound.
		uint ebo = gl.GenBuffer();
		gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
		fixed (uint* i = &mesh.Indices[0])
			gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(mesh.Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);

		uint vbo = gl.GenBuffer();
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (Vertex* v = &mesh.Vertices[0])
			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		gl.EnableVertexAttribArray(1);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		// TODO: We don't do anything with normals here.
		gl.EnableVertexAttribArray(2);
		gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		// Unbind the VAO before unbinding the buffers. Unbinding the element array buffer while the VAO is still bound
		// would clear the VAO's element array binding.
		gl.BindVertexArray(0);
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

		return new GpuMesh(gl, vao, vbo, ebo, (uint)mesh.Indices.Length);
	}

	/// <summary>
	/// Binds this mesh's vertex array object. Use together with <see cref="DrawBound"/> to issue several draws from the
	/// same mesh without rebinding.
	/// </summary>
	public void Bind()
	{
		_gl.BindVertexArray(_vao);
	}

	/// <summary>
	/// Draws this mesh. Requires <see cref="Bind"/> to have been called.
	/// </summary>
	public unsafe void DrawBound()
	{
		_gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, (void*)0);
	}

	/// <summary>
	/// Binds and draws this mesh. Prefer <see cref="Bind"/> + <see cref="DrawBound"/> when drawing the same mesh repeatedly.
	/// </summary>
	public void Draw()
	{
		Bind();
		DrawBound();
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		_gl.DeleteVertexArray(_vao);
		_gl.DeleteBuffer(_vbo);
		_gl.DeleteBuffer(_ebo);
	}
}
