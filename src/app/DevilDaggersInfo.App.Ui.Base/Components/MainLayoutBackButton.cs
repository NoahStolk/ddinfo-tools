using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class MainLayoutBackButton : TooltipIconButton
{
	public MainLayoutBackButton(IBounds bounds, Action onClick)
		: base(bounds, onClick, ButtonStyles.Default, Textures.BackButton, "Back", TextAlign.Left, Color.HalfTransparentWhite, Color.White)
	{
	}
}
