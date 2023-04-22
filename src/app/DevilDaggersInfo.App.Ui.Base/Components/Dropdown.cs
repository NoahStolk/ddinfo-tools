using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class Dropdown : AbstractDropdown
{
	public Dropdown(IBounds bounds, string text, DropdownStyle dropdownStyle)
		: base(bounds)
	{
		Text = text;
		DropdownStyle = dropdownStyle;
	}

	public string Text { get; set; }

	public DropdownStyle DropdownStyle { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> borderVec = new(1);
		Vector2i<int> scale = Bounds.Size;
		Vector2i<int> topLeft = Bounds.TopLeft;
		Vector2i<int> center = topLeft + scale / 2;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + center, Depth, Color.White);
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - borderVec * 2, scrollOffset + center, Depth + 1, Hover ? Color.Gray(0.5f) : Color.Black);

		Vector2i<int> textPosition = new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2;
		Root.Game.GetFontRenderer(DropdownStyle.FontSize).Schedule(new(1), scrollOffset + textPosition, Depth + 2, Color.White, Text, TextAlign.Middle);
	}
}
