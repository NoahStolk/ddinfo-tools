using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

internal sealed class Tile(float positionX, float positionZ, int arenaX, int arenaY, Camera camera, ResourceManager resourceManager)
{
	private static uint _vaoTile;
	private static uint _vaoPillar;
	private static uint _vaoHitbox;

	private readonly TileMeshObject _top = new(_vaoTile, ContentManager.Content.TileMesh, positionX, positionZ);
	private readonly TileMeshObject _pillar = new(_vaoPillar, ContentManager.Content.PillarMesh, positionX, positionZ);
	private readonly TileHitboxMeshObject _tileHitbox = new(_vaoHitbox, resourceManager.InternalResources.TileHitboxModel.MainMesh, positionX, positionZ);

	public float PositionX { get; } = positionX;
	public float Height { get; private set; }
	public float PositionZ { get; } = positionZ;
	public int ArenaX { get; } = arenaX;
	public int ArenaY { get; } = arenaY;

	public static void InitializeRendering(GL gl, ResourceManager resourceManager)
	{
		if (_vaoTile != 0)
			throw new InvalidOperationException("Skull 4 is already initialized.");

		_vaoTile = MeshShaderUtils.CreateVao(gl, ContentManager.Content.TileMesh);
		_vaoPillar = MeshShaderUtils.CreateVao(gl, ContentManager.Content.PillarMesh);
		_vaoHitbox = MeshShaderUtils.CreateVao(gl, resourceManager.InternalResources.TileHitboxModel.MainMesh);
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
