namespace DevilDaggersInfo.App.Ui.Base.Extensions;

public static class WikiColorExtensions
{
	public static Color ToWarpColor(this DevilDaggersInfo.Core.Wiki.Structs.Color c) => new(c.R, c.G, c.B, 255);
}
