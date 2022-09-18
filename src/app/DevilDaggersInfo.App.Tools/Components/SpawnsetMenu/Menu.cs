using DevilDaggersInfo.App.Tools.States;
using DevilDaggersInfo.Core.Spawnset;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetMenu;

public class Menu : AbstractComponent
{
	public Menu(Rectangle metric)
		: base(metric)
	{
		Depth = 100;
		List<AbstractComponent> fileMenuButtons = new()
		{
			new Button.MenuButton(Rectangle.At(0, 24, 96, 24), () => StateManager.SetSpawnset("(untitled)", SpawnsetBinary.CreateDefault()), "New"),
			new Button.MenuButton(Rectangle.At(0, 48, 96, 24), () => Base.Game.ActiveLayout = Base.Game.OpenLayout, "Open"),
			new Button.MenuButton(Rectangle.At(0, 72, 96, 24), () => Base.Game.ActiveLayout = Base.Game.SaveLayout, "Save"),
		};

		Dropdown fileMenu = new(new(0, 0, 96, 96), fileMenuButtons, Color.White, "File")
		{
			Depth = 101,
		};

		NestingContext.Add(fileMenu);
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		Base.Game.UiRenderer.RenderTopLeft(Metric.Size, parentPosition + new Vector2i<int>(Metric.X1, Metric.Y1), Depth, Color.Gray(0.05f));
	}
}
