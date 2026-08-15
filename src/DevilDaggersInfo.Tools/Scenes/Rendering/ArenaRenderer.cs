// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.Rendering;

/// <summary>
/// Draws an <see cref="ArenaScene"/>. This owns every OpenGL call involved in rendering an arena; the scene and its game
/// objects hold only state.
/// </summary>
/// <remarks>
/// A single instance is shared by every scene. That is safe because only one layout renders per frame and all rendering
/// happens on the GL thread, but it means this type must not cache anything scene-specific between frames.
/// </remarks>
internal sealed class ArenaRenderer(GL gl, MeshCache meshCache, ResourceManager resourceManager, ContentManager contentManager)
{
	private ArenaMeshes? _meshes;

	/// <summary>
	/// Uploads the meshes needed to render an arena, so the cost is not paid during the first frame. Optional; they are
	/// resolved on demand otherwise.
	/// </summary>
	public void WarmUpMeshes()
	{
		_ = GetMeshes();
	}

	/// <summary>
	/// Drops the resolved meshes so they are looked up again on the next frame. Must be called whenever the game content
	/// is reloaded, because the previous <see cref="GpuMesh"/> instances are disposed with the cache.
	/// </summary>
	public void InvalidateMeshes()
	{
		_meshes = null;
	}

	/// <summary>
	/// Draws the scene as it currently stands. This only reads scene state; advancing it, including camera matrices and
	/// tile picking, is <see cref="ArenaScene.Update"/>'s job.
	/// </summary>
	public void Render(ArenaScene scene, bool isHovering)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		ArenaMeshes meshes = GetMeshes();

		Shader shader = resourceManager.InternalResources.MeshShader;
		gl.UseProgram(shader.Id);

		SetFrameUniforms(scene, shader);

		resourceManager.GameResources.PostLut.Bind(TextureUnit.Texture1);

		int modelLocation = shader.GetUniformLocation("model");

		RenderTiles(scene, shader, modelLocation, meshes, isHovering);
		RenderRaceDagger(scene, modelLocation, meshes);
		RenderPlayer(scene, modelLocation, meshes);
		RenderSkull4(scene, modelLocation, meshes);
		RenderTileHitboxes(scene, modelLocation, meshes);
	}

	private void SetFrameUniforms(ArenaScene scene, Shader shader)
	{
		gl.UniformMatrix4x4(shader.GetUniformLocation("view"), scene.Camera.ViewMatrix);
		gl.UniformMatrix4x4(shader.GetUniformLocation("projection"), scene.Camera.Projection);
		gl.Uniform1(shader.GetUniformLocation("textureDiffuse"), 0);
		gl.Uniform1(shader.GetUniformLocation("textureLut"), 1);

		// The shader program is shared by every scene, so highlightColor must be reset explicitly. Only the editor path
		// ever writes it, and without this the other paths inherit whatever it was last set to.
		gl.Uniform3(shader.GetUniformLocation("highlightColor"), Vector3.Zero);

		SetLightUniforms(scene, shader);
	}

	private unsafe void SetLightUniforms(ArenaScene scene, Shader shader)
	{
		IReadOnlyList<LightObject> lights = scene.Lights;

		Span<float> positions = stackalloc float[lights.Count * 3];
		Span<float> colors = stackalloc float[lights.Count * 3];
		Span<float> radii = stackalloc float[lights.Count];
		for (int i = 0; i < lights.Count; i++)
		{
			LightObject light = lights[i];

			positions[i * 3] = light.Position.X;
			positions[i * 3 + 1] = light.Position.Y;
			positions[i * 3 + 2] = light.Position.Z;
			colors[i * 3] = light.Color.X;
			colors[i * 3 + 1] = light.Color.Y;
			colors[i * 3 + 2] = light.Color.Z;
			radii[i] = light.Radius;
		}

		// Use the explicit-count overloads so the vec3 count cannot be confused with the float count.
		gl.Uniform1(shader.GetUniformLocation("lightCount"), lights.Count);
		fixed (float* p = positions)
			gl.Uniform3(shader.GetUniformLocation("lightPosition"), (uint)lights.Count, p);
		fixed (float* c = colors)
			gl.Uniform3(shader.GetUniformLocation("lightColor"), (uint)lights.Count, c);
		fixed (float* r = radii)
			gl.Uniform1(shader.GetUniformLocation("lightRadius"), (uint)lights.Count, r);
	}

	private void RenderTiles(ArenaScene scene, Shader shader, int modelLocation, ArenaMeshes meshes, bool isHovering)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		// Tile highlighting only applies in the editor, and only while the shrink preview is not active.
		ArenaEditorContext? editorContext = scene.CurrentTick == 0 ? scene.EditorContext : null;

		int highlightLocation = shader.GetUniformLocation("highlightColor");

		resourceManager.GameResources.TileTexture.Bind();
		RenderTilePass(scene, modelLocation, highlightLocation, meshes.Tile, editorContext, isHovering);

		resourceManager.GameResources.PillarTexture.Bind();
		RenderTilePass(scene, modelLocation, highlightLocation, meshes.Pillar, editorContext, isHovering);
	}

	private void RenderTilePass(ArenaScene scene, int modelLocation, int highlightLocation, GpuMesh mesh, ArenaEditorContext? editorContext, bool isHovering)
	{
		mesh.Bind();

		for (int i = 0; i < scene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < scene.Tiles.GetLength(1); j++)
			{
				Tile tile = scene.Tiles[i, j];
				if (!tile.IsVisible)
					continue;

				Vector3 highlightColor = editorContext?.GetHighlightColor(tile, isHovering) ?? default;
				bool highlight = highlightColor != default;
				if (highlight)
					gl.Uniform3(highlightLocation, highlightColor);

				gl.UniformMatrix4x4(modelLocation, Matrix4x4.CreateTranslation(tile.PositionX, tile.Height, tile.PositionZ));
				mesh.DrawBound();

				if (highlight)
					gl.Uniform3(highlightLocation, Vector3.Zero);
			}
		}
	}

	private void RenderRaceDagger(ArenaScene scene, int modelLocation, ArenaMeshes meshes)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		RaceDagger raceDagger = scene.RaceDagger;

		resourceManager.GameResources.DaggerSilverTexture.Bind();
		gl.UniformMatrix4x4(modelLocation, Matrix4x4.CreateScale(8) * Matrix4x4.CreateFromQuaternion(raceDagger.MeshRotation) * Matrix4x4.CreateTranslation(raceDagger.MeshPosition));
		meshes.Dagger.Draw();
	}

	private void RenderPlayer(ArenaScene scene, int modelLocation, ArenaMeshes meshes)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		Player? player = scene.Player;
		if (player == null)
			return;

		resourceManager.GameResources.Hand4Texture.Bind();
		gl.UniformMatrix4x4(modelLocation, Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(player.Rotation) * Matrix4x4.CreateTranslation(player.Position));
		meshes.Hand.Draw();
	}

	private void RenderSkull4(ArenaScene scene, int modelLocation, ArenaMeshes meshes)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

		if (!scene.ShowSkull4)
			return;

		gl.UniformMatrix4x4(modelLocation, Matrix4x4.CreateScale(1.5f) * Matrix4x4.CreateTranslation(new Vector3(0, 4f, 0)));

		resourceManager.GameResources.Skull4Texture.Bind();
		meshes.Skull4.Draw();

		resourceManager.GameResources.Skull4JawTexture.Bind();
		meshes.Skull4Jaw.Draw();
	}

	/// <summary>
	/// Draws the alpha-blended tile hitboxes. These must be drawn back to front with depth writes disabled, otherwise
	/// each hitbox occludes the ones behind it instead of blending with them.
	/// </summary>
	private void RenderTileHitboxes(ArenaScene scene, int modelLocation, ArenaMeshes meshes)
	{
		int count = scene.CollectVisibleHitboxTiles(out Tile[] tiles);
		if (count == 0)
			return;

		resourceManager.InternalResources.TileHitboxTexture.Bind();

		gl.DepthMask(false);

		meshes.TileHitbox.Bind();
		for (int i = count - 1; i >= 0; i--)
		{
			Tile tile = tiles[i];
			gl.UniformMatrix4x4(modelLocation, CreateHeightScaledTranslation(tile.HitboxHeight, tile.PositionX, tile.HitboxPositionY, tile.PositionZ));
			meshes.TileHitbox.DrawBound();
		}

		// Depth writes must be restored, otherwise the next frame's glClear cannot clear the depth buffer.
		gl.DepthMask(true);
	}

	/// <summary>
	/// Closed form of <c>CreateScale(1, heightScale, 1) * CreateTranslation(x, y, z)</c>, which avoids a matrix multiply
	/// per tile.
	/// </summary>
	private static Matrix4x4 CreateHeightScaledTranslation(float heightScale, float x, float y, float z)
	{
		Matrix4x4 matrix = Matrix4x4.Identity;
		matrix.M22 = heightScale;
		matrix.M41 = x;
		matrix.M42 = y;
		matrix.M43 = z;
		return matrix;
	}

	private ArenaMeshes GetMeshes()
	{
		return _meshes ??= new ArenaMeshes(
			Tile: meshCache.GetOrCreate(contentManager.Content.TileMesh),
			Pillar: meshCache.GetOrCreate(contentManager.Content.PillarMesh),
			TileHitbox: meshCache.GetOrCreate(resourceManager.InternalResources.TileHitboxModel.MainMesh),
			Dagger: meshCache.GetOrCreate(contentManager.Content.DaggerMesh),
			Hand: meshCache.GetOrCreate(contentManager.Content.Hand4Mesh),
			Skull4: meshCache.GetOrCreate(contentManager.Content.Skull4Mesh),
			Skull4Jaw: meshCache.GetOrCreate(contentManager.Content.Skull4JawMesh));
	}

	private sealed record ArenaMeshes(
		GpuMesh Tile,
		GpuMesh Pillar,
		GpuMesh TileHitbox,
		GpuMesh Dagger,
		GpuMesh Hand,
		GpuMesh Skull4,
		GpuMesh Skull4Jaw);
}
