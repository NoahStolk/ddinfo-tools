using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App;

public record GameResources(
	Texture IconDaggerTexture,
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
		Texture iconDaggerTexture = new(gl, ContentManager.Content.IconDaggerTexture.Pixels, (uint)ContentManager.Content.IconDaggerTexture.Width, (uint)ContentManager.Content.IconDaggerTexture.Height);
		Texture daggerSilverTexture = new(gl, ContentManager.Content.DaggerSilverTexture.Pixels, (uint)ContentManager.Content.DaggerSilverTexture.Width, (uint)ContentManager.Content.DaggerSilverTexture.Height);
		Texture skull4Texture = new(gl, ContentManager.Content.Skull4Texture.Pixels, (uint)ContentManager.Content.Skull4Texture.Width, (uint)ContentManager.Content.Skull4Texture.Height);
		Texture skull4JawTexture = new(gl, ContentManager.Content.Skull4JawTexture.Pixels, (uint)ContentManager.Content.Skull4JawTexture.Width, (uint)ContentManager.Content.Skull4JawTexture.Height);
		Texture tileTexture = new(gl, ContentManager.Content.TileTexture.Pixels, (uint)ContentManager.Content.TileTexture.Width, (uint)ContentManager.Content.TileTexture.Height);
		Texture pillarTexture = new(gl, ContentManager.Content.PillarTexture.Pixels, (uint)ContentManager.Content.PillarTexture.Width, (uint)ContentManager.Content.PillarTexture.Height);
		Texture postLut = new(gl, ContentManager.Content.PostLut.Pixels, (uint)ContentManager.Content.PostLut.Width, (uint)ContentManager.Content.PostLut.Height);
		Texture hand4Texture = new(gl, ContentManager.Content.Hand4Texture.Pixels, (uint)ContentManager.Content.Hand4Texture.Width, (uint)ContentManager.Content.Hand4Texture.Height);
		return new(iconDaggerTexture, daggerSilverTexture, skull4Texture, skull4JawTexture, tileTexture, pillarTexture, postLut, hand4Texture);
	}
}
