using DevilDaggersInfo.App.Tools.Enums;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Dropdown : AbstractDropdown
{
	public Dropdown(Rectangle metric, List<AbstractComponent> children, Color textColor, string text)
		: base(metric, children)
	{
		Button button = new(new(0, 0, 96, 24), () => Toggle(!IsOpen), Color.Black, Color.White, Color.Gray(0.75f), textColor, text, TextAlign.Middle, 2, false)
		{
			Depth = 102,
		};
		NestingContext.Add(button);
	}
}
