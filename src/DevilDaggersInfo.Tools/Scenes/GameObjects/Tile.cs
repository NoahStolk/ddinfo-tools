using DevilDaggersInfo.Tools.Engine;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class Tile(float positionX, float positionZ, int arenaX, int arenaY, Camera camera)
{
	private static GpuMesh? _tileMesh;
	private static GpuMesh? _pillarMesh;
	private static GpuMesh? _hitboxMesh;

	private readonly TileMeshObject _top = new(TileMesh, positionX, positionZ);
	private readonly TileMeshObject _pillar = new(PillarMesh, positionX, positionZ);
	private readonly TileHitboxMeshObject _tileHitbox = new(HitboxMesh, positionX, positionZ);

	public float PositionX { get; } = positionX;
	public float Height { get; private set; }
	public float PositionZ { get; } = positionZ;
	public int ArenaX { get; } = arenaX;
	public int ArenaY { get; } = arenaY;

	private static GpuMesh TileMesh => _tileMesh ?? throw new InvalidOperationException("Tile rendering is not initialized.");
	private static GpuMesh PillarMesh => _pillarMesh ?? throw new InvalidOperationException("Tile rendering is not initialized.");
	private static GpuMesh HitboxMesh => _hitboxMesh ?? throw new InvalidOperationException("Tile rendering is not initialized.");

	public static void InitializeRendering(GL gl, ResourceManager resourceManager)
	{
		if (_tileMesh != null)
			throw new InvalidOperationException("Tile is already initialized.");

		_tileMesh = GpuMesh.Create(gl, ContentManager.Content.TileMesh);
		_pillarMesh = GpuMesh.Create(gl, ContentManager.Content.PillarMesh);
		_hitboxMesh = GpuMesh.Create(gl, resourceManager.InternalResources.TileHitboxModel.MainMesh);
	}

	public float SquaredDistanceToCamera()
	{
		return Vector2.DistanceSquared(new Vector2(PositionX, PositionZ), new Vector2(camera.Position.X, camera.Position.Z));
	}

	public void SetDisplayHeight(float height)
	{
		Height = height;

		_top.PositionY = Height;
		_pillar.PositionY = Height;

		const float tileMeshHeight = 4;
		_tileHitbox.PositionY = Height - tileMeshHeight / 2;

		const float tileHitboxOffset = 1;
		_tileHitbox.Height = Height - tileMeshHeight / 2 + tileHitboxOffset;
	}

	public void RenderTop(GL gl, ResourceManager resourceManager)
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight)
			return;

		_top.Render(gl, resourceManager);
	}

	public void RenderPillar(GL gl, ResourceManager resourceManager)
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight)
			return;

		_pillar.Render(gl, resourceManager);
	}

	public void RenderHitbox(GL gl, ResourceManager resourceManager)
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight + 2)
			return;

		_tileHitbox.Render(gl, resourceManager);
	}
}
