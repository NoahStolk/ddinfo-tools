using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class Skull4
{
	private static uint _vaoMain;
	private static uint _vaoJaw;

	public static void InitializeRendering()
	{
		if (_vaoMain != 0)
			throw new InvalidOperationException("Skull 4 is already initialized.");

		_vaoMain = MeshShaderUtils.CreateVao(ContentManager.Content.Skull4Mesh);
		_vaoJaw = MeshShaderUtils.CreateVao(ContentManager.Content.Skull4JawMesh);
	}

	public unsafe void Render()
	{
		Root.InternalResources.MeshShader.SetUniform("model", Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new(0, 4f, 0)));

		Root.GameResources.Skull4Texture.Bind();

		Root.Gl.BindVertexArray(_vaoMain);
		fixed (uint* i = &ContentManager.Content.Skull4Mesh.Indices[0])
			Root.Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		Root.GameResources.Skull4JawTexture.Bind();

		Root.Gl.BindVertexArray(_vaoJaw);
		fixed (uint* i = &ContentManager.Content.Skull4JawMesh.Indices[0])
			Root.Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4JawMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		Root.Gl.BindVertexArray(0);
	}
}
