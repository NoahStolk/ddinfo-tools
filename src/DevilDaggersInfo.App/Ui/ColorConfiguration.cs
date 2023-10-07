using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Ui;

public struct ColorConfiguration
{
	public required Color Primary { get; init; }
	public required Color Secondary { get; init; }
	public required Color Tertiary { get; init; }
	public required Color Quaternary { get; init; }
}
