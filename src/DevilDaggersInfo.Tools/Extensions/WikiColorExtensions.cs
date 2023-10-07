using DevilDaggersInfo.Tools.Engine.Maths.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class WikiColorExtensions
{
	public static Color ToEngineColor(this DevilDaggersInfo.Core.Wiki.Structs.Color c) => new(c.R, c.G, c.B, 255);
}
