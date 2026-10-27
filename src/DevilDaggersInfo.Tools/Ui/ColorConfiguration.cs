using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui;

internal record struct ColorConfiguration
{
	public required Vector4 Primary { get; init; }
	public required Vector4 Secondary { get; init; }
	public required Vector4 Tertiary { get; init; }
	public required Vector4 Quaternary { get; init; }
}
