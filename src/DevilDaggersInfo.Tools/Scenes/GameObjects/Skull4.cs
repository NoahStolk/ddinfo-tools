using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

public sealed class Skull4(GL gl, ResourceManager resourceManager)
{
	private static uint _vaoMain;
	private static uint _vaoJaw;

	public static void InitializeRendering(GL gl)
	{
		if (_vaoMain != 0)
			throw new InvalidOperationException("Skull 4 is already initialized.");

		_vaoMain = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Skull4Mesh);
		_vaoJaw = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Skull4JawMesh);
	}

	public unsafe void Render()
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new Vector3(0, 4f, 0)));

		resourceManager.GameResources.Skull4Texture.Bind();

		gl.BindVertexArray(_vaoMain);
		fixed (uint* i = &ContentManager.Content.Skull4Mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		resourceManager.GameResources.Skull4JawTexture.Bind();

		gl.BindVertexArray(_vaoJaw);
		fixed (uint* i = &ContentManager.Content.Skull4JawMesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4JawMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		gl.BindVertexArray(0);
	}
}
