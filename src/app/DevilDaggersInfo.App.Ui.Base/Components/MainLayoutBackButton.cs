using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Styling;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class MainLayoutBackButton : TooltipIconButton
{
	public MainLayoutBackButton(IBounds bounds, Action onClick)
		: base(bounds, onClick, ButtonStyles.Default, Textures.BackButton, "Back", TextAlign.Left, Color.HalfTransparentWhite, Color.White)
	{
	}
}
