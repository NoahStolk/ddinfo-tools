using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Components;

public class Menu : AbstractComponent
{
	private const int _headerHeight = 24;

	public Menu(IBounds bounds)
		: base(bounds)
	{
		const int backButtonWidth = 24;

		Depth = 100;

		MainLayoutBackButton backButton = new(new PixelBounds(0, 0, backButtonWidth, _headerHeight), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.MainLayout)))
		{
			Depth = 101,
		};

		const int menuItemWidth = 160;
		const int menuItemHeight = 16;

		Dropdown fileMenu = new(new PixelBounds(backButtonWidth, 0, 64, _headerHeight), "File", DropdownStyles.Default)
		{
			Depth = 101,
		};

		const int dropdownEntryDepth = 102;
		DropdownEntry entryNew = new(new PixelBounds(fileMenu.Bounds.X1, _headerHeight + menuItemHeight * 0, menuItemWidth, menuItemHeight), fileMenu, () => StateManager.Dispatch(new LoadSpawnset("(untitled)", SpawnsetBinary.CreateDefault())), "New", DropdownEntryStyles.Default) { Depth = dropdownEntryDepth };
		DropdownEntry entryOpen = new(new PixelBounds(fileMenu.Bounds.X1, _headerHeight + menuItemHeight * 1, menuItemWidth, menuItemHeight), fileMenu, SpawnsetFileUtils.OpenSpawnset, "Open", DropdownEntryStyles.Default) { Depth = dropdownEntryDepth };
		DropdownEntry entryOpenDefault = new(new PixelBounds(fileMenu.Bounds.X1, _headerHeight + menuItemHeight * 2, menuItemWidth, menuItemHeight), fileMenu, () => StateManager.Dispatch(new LoadSpawnset("(untitled)", ContentManager.Content.DefaultSpawnset.DeepCopy())), "Open default (V3)", DropdownEntryStyles.Default) { Depth = dropdownEntryDepth };
		DropdownEntry entrySave = new(new PixelBounds(fileMenu.Bounds.X1, _headerHeight + menuItemHeight * 3, menuItemWidth, menuItemHeight), fileMenu, SpawnsetFileUtils.SaveSpawnset, "Save", DropdownEntryStyles.Default) { Depth = dropdownEntryDepth };
		DropdownEntry entryReplace = new(new PixelBounds(fileMenu.Bounds.X1, _headerHeight + menuItemHeight * 4, menuItemWidth, menuItemHeight), fileMenu, SpawnsetFileUtils.ReplaceSpawnset, "Replace", DropdownEntryStyles.Default) { Depth = dropdownEntryDepth };

		fileMenu.AddChild(entryNew);
		fileMenu.AddChild(entryOpen);
		fileMenu.AddChild(entryOpenDefault);
		fileMenu.AddChild(entrySave);
		fileMenu.AddChild(entryReplace);

		NestingContext.Add(entryNew);
		NestingContext.Add(entryOpen);
		NestingContext.Add(entryOpenDefault);
		NestingContext.Add(entrySave);
		NestingContext.Add(entryReplace);

		NestingContext.Add(backButton);
		NestingContext.Add(fileMenu);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.RectangleRenderer.Schedule(new(Bounds.Size.X, _headerHeight), scrollOffset + Bounds.TopLeft + new Vector2i<int>(Bounds.Size.X, _headerHeight) / 2, Depth, Color.Gray(0.05f));
	}
}
