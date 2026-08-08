namespace DevilDaggersInfo.Tools;

internal sealed record GameResources(
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
	Texture Hand4Texture) : IDisposable
{
	/// <summary>
	/// Deletes every texture in this set. Only valid while a GL context is current, so this is called when the game
	/// content is reloaded rather than at container teardown.
	/// </summary>
	public void Dispose()
	{
		IconMaskCrosshairTexture.Dispose();
		IconMaskDaggerTexture.Dispose();
		IconMaskGemTexture.Dispose();
		IconMaskHomingTexture.Dispose();
		IconMaskSkullTexture.Dispose();
		IconMaskStopwatchTexture.Dispose();
		DaggerSilverTexture.Dispose();
		Skull4Texture.Dispose();
		Skull4JawTexture.Dispose();
		TileTexture.Dispose();
		PillarTexture.Dispose();
		PostLut.Dispose();
		Hand4Texture.Dispose();
	}
}
