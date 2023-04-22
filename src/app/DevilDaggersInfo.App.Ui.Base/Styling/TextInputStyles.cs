using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;

namespace DevilDaggersInfo.App.Ui.Base.Styling;

public static class TextInputStyles
{
	public static TextInputStyle Default { get; } = new(Color.Black, Color.Gray(0.75f), Color.Gray(0.25f), Color.White, Color.White, Color.Green, Color.Gray(0.5f), 2, 4, FontSize.H12);
}
