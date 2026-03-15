using DevilDaggersInfo.Tools.Engine.Content;
using DevilDaggersInfo.Tools.Extensions;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class TileMeshObject(uint vao, MeshContent mesh, float positionX, float positionZ)
{
	public float PositionY { get; set; }

	public unsafe void Render(GL gl, ResourceManager resourceManager)
	{
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(positionX, PositionY, positionZ));
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), translationMatrix);

		gl.BindVertexArray(vao);
		fixed (uint* i = &mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
		gl.BindVertexArray(0);
	}
}
