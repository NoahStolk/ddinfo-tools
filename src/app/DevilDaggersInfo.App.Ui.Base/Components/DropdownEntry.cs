using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class DropdownEntry : AbstractDropdownEntry
{
	public DropdownEntry(IBounds bounds, AbstractDropdown parent, Action onClick, string text, DropdownEntryStyle dropdownEntryStyle)
		: base(bounds, parent, onClick)
	{
		Text = text;
		DropdownEntryStyle = dropdownEntryStyle;
		IsActive = false;
	}

	public string Text { get; set; }

	public DropdownEntryStyle DropdownEntryStyle { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> borderVec = new(1);
		Vector2i<int> scale = Bounds.Size;
		Vector2i<int> topLeft = Bounds.TopLeft;
		Vector2i<int> center = topLeft + scale / 2;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + center, Depth, Color.White);
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - borderVec * 2, scrollOffset + center, Depth + 1, Hover && !IsDisabled ? Color.Gray(0.5f) : Color.Black);

		int padding = (int)MathF.Round(Bounds.Size.Y / 4f);
		Vector2i<int> textPosition = DropdownEntryStyle.TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2,
			TextAlign.Left => new(Bounds.X1 + padding, Bounds.Y1 + padding),
			TextAlign.Right => new(Bounds.X2 - padding, Bounds.Y1 + padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};
		Root.Game.GetFontRenderer(DropdownEntryStyle.FontSize).Schedule(new(1), scrollOffset + textPosition, Depth + 2, Color.White, Text, DropdownEntryStyle.TextAlign);
	}
}
