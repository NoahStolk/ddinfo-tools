using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal sealed unsafe class ArenaSceneRendering(GL gl, ResourceManager resourceManager)
{
	private uint _vaoPlayer;

	private uint _vaoRaceDagger;

	private uint _vaoTile;
	private uint _vaoPillar;
	private uint _vaoTileHitbox;

	private uint _vaoSkull4Main;
	private uint _vaoSkull4Jaw;

	public void InitializeRendering()
	{
		_vaoPlayer = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Hand4Mesh);

		_vaoRaceDagger = MeshShaderUtils.CreateVao(gl, ContentManager.Content.DaggerMesh);

		_vaoTile = MeshShaderUtils.CreateVao(gl, ContentManager.Content.TileMesh);
		_vaoPillar = MeshShaderUtils.CreateVao(gl, ContentManager.Content.PillarMesh);
		_vaoTileHitbox = MeshShaderUtils.CreateVao(gl, resourceManager.InternalResources.TileHitboxModel.MainMesh);

		_vaoSkull4Main = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Skull4Mesh);
		_vaoSkull4Jaw = MeshShaderUtils.CreateVao(gl, ContentManager.Content.Skull4JawMesh);
	}

	public void RenderPlayer(Player player)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		resourceManager.GameResources.Hand4Texture.Bind();
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(player.PlayerMovement.Rotation) * Matrix4x4.CreateTranslation(player.PlayerMovement.Position));

		gl.BindVertexArray(_vaoPlayer);
		fixed (uint* i = &ContentManager.Content.Hand4Mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Hand4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		gl.BindVertexArray(0);
	}

	public void RenderDagger(RaceDagger raceDagger)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		resourceManager.GameResources.DaggerSilverTexture.Bind();
		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(8) * Matrix4x4.CreateFromQuaternion(raceDagger.MeshRotation) * Matrix4x4.CreateTranslation(raceDagger.MeshPosition));

		gl.BindVertexArray(_vaoRaceDagger);
		fixed (uint* i = &ContentManager.Content.DaggerMesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.DaggerMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		gl.BindVertexArray(0);
	}

	public void RenderSkull4()
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		gl.UniformMatrix4x4(resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new Vector3(0, 4f, 0)));

		resourceManager.GameResources.Skull4Texture.Bind();

		gl.BindVertexArray(_vaoSkull4Main);
		fixed (uint* i = &ContentManager.Content.Skull4Mesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		resourceManager.GameResources.Skull4JawTexture.Bind();

		gl.BindVertexArray(_vaoSkull4Jaw);
		fixed (uint* i = &ContentManager.Content.Skull4JawMesh.Indices[0])
			gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.Skull4JawMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		gl.BindVertexArray(0);
	}
}
