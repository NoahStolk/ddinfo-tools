using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class TileMeshObject(GpuMesh mesh, float positionX, float positionZ)
{
	public float PositionY { get; set; }

	public void Render(GL gl, ResourceManager resourceManager)
	{
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(positionX, PositionY, positionZ));
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), translationMatrix);

		mesh.Draw();
	}
}
