using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App;

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
	public static GameResources Create(GL gl)
	{
		return new(
			IconMaskCrosshairTexture: new(gl, ContentManager.Content.IconMaskCrosshairTexture.Pixels, (uint)ContentManager.Content.IconMaskCrosshairTexture.Width, (uint)ContentManager.Content.IconMaskCrosshairTexture.Height),
			IconMaskDaggerTexture: new(gl, ContentManager.Content.IconMaskDaggerTexture.Pixels, (uint)ContentManager.Content.IconMaskDaggerTexture.Width, (uint)ContentManager.Content.IconMaskDaggerTexture.Height),
			IconMaskGemTexture: new(gl, ContentManager.Content.IconMaskGemTexture.Pixels, (uint)ContentManager.Content.IconMaskGemTexture.Width, (uint)ContentManager.Content.IconMaskGemTexture.Height),
			IconMaskHomingTexture: new(gl, ContentManager.Content.IconMaskHomingTexture.Pixels, (uint)ContentManager.Content.IconMaskHomingTexture.Width, (uint)ContentManager.Content.IconMaskHomingTexture.Height),
			IconMaskSkullTexture: new(gl, ContentManager.Content.IconMaskSkullTexture.Pixels, (uint)ContentManager.Content.IconMaskSkullTexture.Width, (uint)ContentManager.Content.IconMaskSkullTexture.Height),
			IconMaskStopwatchTexture: new(gl, ContentManager.Content.IconMaskStopwatchTexture.Pixels, (uint)ContentManager.Content.IconMaskStopwatchTexture.Width, (uint)ContentManager.Content.IconMaskStopwatchTexture.Height),
			DaggerSilverTexture: new(gl, ContentManager.Content.DaggerSilverTexture.Pixels, (uint)ContentManager.Content.DaggerSilverTexture.Width, (uint)ContentManager.Content.DaggerSilverTexture.Height),
			Skull4Texture: new(gl, ContentManager.Content.Skull4Texture.Pixels, (uint)ContentManager.Content.Skull4Texture.Width, (uint)ContentManager.Content.Skull4Texture.Height),
			Skull4JawTexture: new(gl, ContentManager.Content.Skull4JawTexture.Pixels, (uint)ContentManager.Content.Skull4JawTexture.Width, (uint)ContentManager.Content.Skull4JawTexture.Height),
			TileTexture: new(gl, ContentManager.Content.TileTexture.Pixels, (uint)ContentManager.Content.TileTexture.Width, (uint)ContentManager.Content.TileTexture.Height),
			PillarTexture: new(gl, ContentManager.Content.PillarTexture.Pixels, (uint)ContentManager.Content.PillarTexture.Width, (uint)ContentManager.Content.PillarTexture.Height),
			PostLut: new(gl, ContentManager.Content.PostLut.Pixels, (uint)ContentManager.Content.PostLut.Width, (uint)ContentManager.Content.PostLut.Height),
			Hand4Texture: new(gl, ContentManager.Content.Hand4Texture.Pixels, (uint)ContentManager.Content.Hand4Texture.Width, (uint)ContentManager.Content.Hand4Texture.Height));
	}
}
