// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Scene;
using DevilDaggersInfo.App.Ui.Scene.GameObjects;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Scenes;

public sealed class MainMenuArenaScene : IArenaScene
{
	private Skull4? _skull4;

	public Camera Camera { get; } = new();
	public List<Tile> Tiles { get; } = new();
	public List<LightObject> Lights { get; } = new();
	public RaceDagger? RaceDagger { get; set; }

	private void Clear()
	{
		_skull4 = null;

		Tiles.Clear();
		Lights.Clear();

		RaceDagger = null;
	}

	public void BuildSpawnset(SpawnsetBinary spawnset)
	{
		Clear();

		IArenaScene scene = this;
		scene.AddArena(spawnset);

		Camera.IsMenuCamera = true;
		_skull4 = new();
	}

	public void Update(int currentTick)
	{
		for (int i = 0; i < Lights.Count; i++)
			Lights[i].PrepareUpdate();

		Camera.Update();
		RaceDagger?.Update(currentTick);
	}

	public void Render()
	{
		for (int i = 0; i < Lights.Count; i++)
			Lights[i].PrepareRender();

		Camera.PreRender();

		MeshShader.Use();
		MeshShader.SetView(Camera.ViewMatrix);
		MeshShader.SetProjection(Camera.Projection);
		MeshShader.SetTextureDiffuse(0);
		MeshShader.SetTextureLut(1);
		MeshShader.SetLutScale(1f);

		// TODO: Prevent allocating memory.
		Span<Vector3> lightPositions = Lights.Select(lo => lo.PositionState.Render).ToArray();
		Span<Vector3> lightColors = Lights.Select(lo => lo.ColorState.Render).ToArray();
		Span<float> lightRadii = Lights.Select(lo => lo.RadiusState.Render).ToArray();

		MeshShader.SetLightCount(lightPositions.Length);
		MeshShader.SetLightPosition(lightPositions);
		MeshShader.SetLightColor(lightColors);
		MeshShader.SetLightRadius(lightRadii);

		ContentManager.Content.PostLut.Use(TextureUnit.Texture1);

		ContentManager.Content.TileTexture.Use();

		for (int i = 0; i < Tiles.Count; i++)
			Tiles[i].RenderTop();

		ContentManager.Content.PillarTexture.Use();
		for (int i = 0; i < Tiles.Count; i++)
			Tiles[i].RenderPillar();

		RaceDagger?.Render();
		_skull4?.Render();

		Textures.TileHitbox.Use();

		Tiles.Sort(static (a, b) => a.SquaredDistanceToCamera().CompareTo(b.SquaredDistanceToCamera()));
		foreach (Tile tile in Tiles)
			tile.RenderHitbox();
	}
}
