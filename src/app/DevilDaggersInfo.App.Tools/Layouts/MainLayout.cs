using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Enums;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class MainLayout : Layout, IExtendedLayout
{
	public MainLayout()
		: base(new(0, 0, 1920, 1080))
	{
		Color ddse = Color.FromHsv(0, 1, 0.8f);
		Color ddae = Color.FromHsv(130, 1, 0.6f);
		Color ddre = Color.FromHsv(220, 1, 1);
		Color ddcl = Color.FromHsv(270, 1, 1);

		const int border = 10;
		NestingContext.Add(new Button(Rectangle.At(0256, 256, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddse.Intensify(64), ddse, ddse.Intensify(96), Color.White, "Survival Editor", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(1280, 256, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddcl.Intensify(64), ddcl, ddcl.Intensify(96), Color.White, "Custom Leaderboards", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(0256, 704, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddae.Intensify(64), ddae, ddae.Intensify(96), Color.White, "Asset Editor", TextAlign.Middle, border, false));
		NestingContext.Add(new Button(Rectangle.At(1280, 704, 320, 128), () => Root.Game.ActiveLayout = Root.Game.SurvivalEditorMainLayout, ddre.Intensify(64), ddre, ddre.Intensify(96), Color.White, "Replay Editor", TextAlign.Middle, border, false));
	}

	public void Update()
	{
	}

	public void Render()
	{
	}
}
