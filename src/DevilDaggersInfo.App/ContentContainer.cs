using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App;

public record ContentContainer(
	SpawnsetBinary DefaultSpawnset,
	TextureContent IconMaskCrosshairTexture,
	TextureContent IconMaskDaggerTexture,
	TextureContent IconMaskGemTexture,
	TextureContent IconMaskHomingTexture,
	TextureContent IconMaskSkullTexture,
	TextureContent IconMaskStopwatchTexture,
	MeshContent DaggerMesh,
	TextureContent DaggerSilverTexture,
	MeshContent Skull4Mesh,
	TextureContent Skull4Texture,
	MeshContent Skull4JawMesh,
	TextureContent Skull4JawTexture,
	MeshContent TileMesh,
	TextureContent TileTexture,
	MeshContent PillarMesh,
	TextureContent PillarTexture,
	SoundContent SoundJump1,
	SoundContent SoundJump2,
	SoundContent SoundJump3,
	TextureContent PostLut,
	MeshContent Hand4Mesh,
	TextureContent Hand4Texture);
