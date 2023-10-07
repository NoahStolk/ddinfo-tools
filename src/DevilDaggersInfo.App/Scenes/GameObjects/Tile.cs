using System.Numerics;

namespace DevilDaggersInfo.App.Scenes.GameObjects;

public class Tile
{
	private static uint _vaoTile;
	private static uint _vaoPillar;
	private static uint _vaoHitbox;

	private readonly Camera _camera;

	private readonly TileMeshObject _top;
	private readonly TileMeshObject _pillar;
	private readonly TileHitboxMeshObject _tileHitbox;

	public Tile(float positionX, float positionZ, int arenaX, int arenaY, Camera camera)
	{
		PositionX = positionX;
		PositionZ = positionZ;
		ArenaX = arenaX;
		ArenaY = arenaY;
		_camera = camera;

		_top = new(_vaoTile, ContentManager.Content.TileMesh, positionX, positionZ);
		_pillar = new(_vaoPillar, ContentManager.Content.PillarMesh, positionX, positionZ);
		_tileHitbox = new(_vaoHitbox, Root.InternalResources.TileHitboxModel.MainMesh, positionX, positionZ);
	}

	public float PositionX { get; }
	public float Height { get; private set; }
	public float PositionZ { get; }
	public int ArenaX { get; }
	public int ArenaY { get; }

	public static void InitializeRendering()
	{
		if (_vaoTile != 0)
			throw new InvalidOperationException("Skull 4 is already initialized.");

		_vaoTile = MeshShaderUtils.CreateVao(ContentManager.Content.TileMesh);
		_vaoPillar = MeshShaderUtils.CreateVao(ContentManager.Content.PillarMesh);
		_vaoHitbox = MeshShaderUtils.CreateVao(Root.InternalResources.TileHitboxModel.MainMesh);
	}

	public float SquaredDistanceToCamera() => Vector2.DistanceSquared(new(PositionX, PositionZ), new(_camera.Position.X, _camera.Position.Z));

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

	public void RenderTop()
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight)
			return;

		_top.Render();
	}

	public void RenderPillar()
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight)
			return;

		_pillar.Render();
	}

	public void RenderHitbox()
	{
		if (_top.PositionY < ArenaScene.MinRenderTileHeight + 2)
			return;

		_tileHitbox.Render();
	}
}
