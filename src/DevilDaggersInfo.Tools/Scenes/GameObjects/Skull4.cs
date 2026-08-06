using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class Skull4(GL gl, ResourceManager resourceManager)
{
	private static GpuMesh? _mainMesh;
	private static GpuMesh? _jawMesh;

	public static void InitializeRendering(GL gl)
	{
		if (_mainMesh != null)
			throw new InvalidOperationException("Skull 4 is already initialized.");

		_mainMesh = GpuMesh.Create(gl, ContentManager.Content.Skull4Mesh);
		_jawMesh = GpuMesh.Create(gl, ContentManager.Content.Skull4JawMesh);
	}

	public void Render()
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		if (_mainMesh == null || _jawMesh == null)
			throw new InvalidOperationException("Skull 4 rendering is not initialized.");

		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new Vector3(0, 4f, 0)));

		resourceManager.GameResources.Skull4Texture.Bind();
		_mainMesh.Draw();

		resourceManager.GameResources.Skull4JawTexture.Bind();
		_jawMesh.Draw();
	}
}
