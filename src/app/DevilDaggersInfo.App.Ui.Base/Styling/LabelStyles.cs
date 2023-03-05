using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using Warp.NET.Text;

namespace DevilDaggersInfo.App.Ui.Base.Styling;

public static class LabelStyles
{
	public static LabelStyle DefaultLeft { get; } = new(Color.White, TextAlign.Left, FontSize.H12, 4);
	public static LabelStyle DefaultMiddle { get; } = new(Color.White, TextAlign.Middle, FontSize.H12, 4);
	public static LabelStyle DefaultRight { get; } = new(Color.White, TextAlign.Right, FontSize.H12, 4);

	public static LabelStyle Title { get; } = new(Color.White, TextAlign.Middle, FontSize.H24, 12);
}
