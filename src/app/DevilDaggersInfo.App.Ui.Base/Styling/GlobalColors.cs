using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Ui.Base.Styling;

public static class GlobalColors
{
	public static Color EntrySelectHover { get; } = new(0, 127, 255, 127);
	public static Color EntrySelect { get; } = new(0, 127, 255, 63);
	public static Color EntryHover { get; } = Color.Gray(0.2f);

	public static Color ShrinkStart => Color.Blue;
	public static Color ShrinkCurrent => Color.Purple;
	public static Color ShrinkEnd => Color.Red;
}
