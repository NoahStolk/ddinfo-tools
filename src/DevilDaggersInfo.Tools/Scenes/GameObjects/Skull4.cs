// using DevilDaggersInfo.Tools.Engine;
// using DevilDaggersInfo.Tools.Extensions;
// using Silk.NET.OpenGL;
// using System.Numerics;
//
// namespace DevilDaggersInfo.Tools.Scenes.GameObjects;
//
// public class Skull4
// {
// 	private static uint _vaoMain;
// 	private static uint _vaoJaw;
//
// 	public static void InitializeRendering()
// 	{
// 		if (_vaoMain != 0)
// 			throw new InvalidOperationException("Skull 4 is already initialized.");
//
// 		_vaoMain = MeshShaderUtils.CreateVao(ContentManager.Content.Skull4Mesh);
// 		_vaoJaw = MeshShaderUtils.CreateVao(ContentManager.Content.Skull4JawMesh);
// 	}
//
// 	public unsafe void Render()
// 	{
// 		Graphics.Gl.UniformMatrix4x4(Root.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new Vector3(0, 4f, 0)));
//
// 		Root.GameResources.Skull4Texture.Bind();
//
// 		Graphics.Gl.BindVertexArray(_vaoMain);
// 		fixed (uint* i = &ContentManager.Content.Skull4Mesh.Indices[0])
// 			Graphics.Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
//
// 		Root.GameResources.Skull4JawTexture.Bind();
//
// 		Graphics.Gl.BindVertexArray(_vaoJaw);
// 		fixed (uint* i = &ContentManager.Content.Skull4JawMesh.Indices[0])
// 			Graphics.Gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4JawMesh.Indices.Length, DrawElementsType.UnsignedInt, i);
//
// 		Graphics.Gl.BindVertexArray(0);
// 	}
// }
