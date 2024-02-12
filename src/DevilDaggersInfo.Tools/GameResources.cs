using DevilDaggersInfo.Tools.Engine.Loaders;

namespace DevilDaggersInfo.Tools;

public record GameResources(
	Texture IconMaskCrosshairTexture,
	Texture IconMaskDaggerTexture,
	Texture IconMaskGemTexture,
	Texture IconMaskHomingTexture,
	Texture IconMaskSkullTexture,
	Texture IconMaskStopwatchTexture,
	Texture DaggerSilverTexture,
	Texture Skull4Texture,
	Texture Skull4JawTexture,
	Texture TileTexture,
	Texture PillarTexture,
	Texture PostLut,
	Texture Hand4Texture)
{
	public static GameResources Create()
	{
		return new(
			IconMaskCrosshairTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskCrosshairTexture)),
			IconMaskDaggerTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskDaggerTexture)),
			IconMaskGemTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskGemTexture)),
			IconMaskHomingTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskHomingTexture)),
			IconMaskSkullTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskSkullTexture)),
			IconMaskStopwatchTexture: new(TextureLoader.Load(ContentManager.Content.IconMaskStopwatchTexture)),
			DaggerSilverTexture: new(TextureLoader.Load(ContentManager.Content.DaggerSilverTexture)),
			Skull4Texture: new(TextureLoader.Load(ContentManager.Content.Skull4Texture)),
			Skull4JawTexture: new(TextureLoader.Load(ContentManager.Content.Skull4JawTexture)),
			TileTexture: new(TextureLoader.Load(ContentManager.Content.TileTexture)),
			PillarTexture: new(TextureLoader.Load(ContentManager.Content.PillarTexture)),
			PostLut: new(TextureLoader.Load(ContentManager.Content.PostLut)),
			Hand4Texture: new(TextureLoader.Load(ContentManager.Content.Hand4Texture)));
	}
}
