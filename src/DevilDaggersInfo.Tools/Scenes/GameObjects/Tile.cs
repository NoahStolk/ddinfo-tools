using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes.GameObjects;

/// <summary>
/// State for a single arena tile. Holds no GPU resources and issues no draw calls; see
/// <see cref="Rendering.ArenaRenderer"/>.
/// </summary>
internal sealed class Tile(float positionX, float positionZ, int arenaX, int arenaY, Camera camera)
{
	private const float _meshHeight = 4;
	private const float _hitboxOffset = 1;

	public float PositionX { get; } = positionX;
	public float PositionZ { get; } = positionZ;
	public float Height { get; private set; }
	public int ArenaX { get; } = arenaX;
	public int ArenaY { get; } = arenaY;

	/// <summary>The vertical centre of this tile's hitbox.</summary>
	public float HitboxPositionY { get; private set; }

	/// <summary>The vertical scale of this tile's hitbox.</summary>
	public float HitboxHeight { get; private set; }

	public bool IsVisible => Height >= ArenaScene.MinRenderTileHeight;

	public bool IsHitboxVisible => Height >= ArenaScene.MinRenderTileHeight + 2;

	public void SetDisplayHeight(float height)
	{
		Height = height;

		HitboxPositionY = height - _meshHeight / 2;
		HitboxHeight = height - _meshHeight / 2 + _hitboxOffset;
	}

	public float SquaredDistanceToCamera()
	{
		return Vector2.DistanceSquared(new Vector2(PositionX, PositionZ), new Vector2(camera.Position.X, camera.Position.Z));
	}
}
