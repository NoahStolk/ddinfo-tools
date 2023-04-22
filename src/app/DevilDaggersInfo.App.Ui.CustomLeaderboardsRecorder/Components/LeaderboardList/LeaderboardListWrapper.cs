using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.Base.User.Cache;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListWrapper : AbstractComponent
{
	private readonly TooltipIconButton _firstButton;
	private readonly TooltipIconButton _prevButton;
	private readonly TooltipIconButton _nextButton;
	private readonly TooltipIconButton _lastButton;

	private readonly LeaderboardListView _leaderboardListView;

	public LeaderboardListWrapper(IBounds bounds)
		: base(bounds)
	{
		int pagingComponentsDepth = Depth + 100;

		TooltipIconButton refreshButton = new(bounds.CreateNested(4, 32, 20, 20), () => StateManager.Dispatch(new ReloadAllCustomLeaderboards()), ButtonStyles.NavigationButton, Textures.Reload, "Reload all", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { Depth = pagingComponentsDepth };
		_firstButton = new(bounds.CreateNested(24, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(0)), ButtonStyles.NavigationButton, Textures.ArrowStart, "First", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { Depth = pagingComponentsDepth };
		_prevButton = new(bounds.CreateNested(44, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.PageIndex - 1)), ButtonStyles.NavigationButton, Textures.ArrowLeft, "Previous", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { Depth = pagingComponentsDepth };
		_nextButton = new(bounds.CreateNested(64, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.PageIndex + 1)), ButtonStyles.NavigationButton, Textures.ArrowRight, "Next", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { Depth = pagingComponentsDepth };
		_lastButton = new(bounds.CreateNested(84, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.GetTotalPages() - 1)), ButtonStyles.NavigationButton, Textures.ArrowEnd, "Last", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { Depth = pagingComponentsDepth };
		NestingContext.Add(refreshButton);
		NestingContext.Add(_firstButton);
		NestingContext.Add(_prevButton);
		NestingContext.Add(_nextButton);
		NestingContext.Add(_lastButton);

		Dropdown categoryDropdown = new(bounds.CreateNested(108, 32, 96, 20), "Category", DropdownStyles.Default) { Depth = Depth + 1 };
		NestingContext.Add(categoryDropdown);
		CustomLeaderboardCategory[] categories = Enum.GetValues<CustomLeaderboardCategory>();
		for (int i = 0; i < categories.Length; i++)
		{
			CustomLeaderboardCategory category = categories[i];
			DropdownEntry dropdownEntry = new(categoryDropdown.Bounds.CreateNested(0, (i + 1) * 20, 96, 20), categoryDropdown, () => StateManager.Dispatch(new SetCategory(category)), category.ToString(), DropdownEntryStyles.Default)
			{
				IsActive = false,
				Depth = pagingComponentsDepth,
			};

			categoryDropdown.AddChild(dropdownEntry);
			NestingContext.Add(dropdownEntry);
		}

		TextInput spawnsetFilterInput = new(bounds.CreateNested(208, 32, 96, 20), false, null, null, s => StateManager.Dispatch(new SetSpawnsetFilter(s)), TextInputStyles.Default);
		TextInput authorFilterInput = new(bounds.CreateNested(308, 32, 96, 20), false, null, null, s => StateManager.Dispatch(new SetAuthorFilter(s)), TextInputStyles.Default);
		Checkbox featuredCheckbox = new(bounds.CreateNested(408, 32, 20, 20), b => StateManager.Dispatch(new SetFeaturedOnly(b)), new(6, 4, 4));
		NestingContext.Add(spawnsetFilterInput);
		NestingContext.Add(authorFilterInput);
		NestingContext.Add(featuredCheckbox);

		const int offsetY = 64;
		_leaderboardListView = new(bounds.CreateNested(0, offsetY, bounds.Size.X, bounds.Size.Y - offsetY));
		NestingContext.Add(_leaderboardListView);

		StateManager.Subscribe<CustomLeaderboardsLoaded>(SetPage);
		StateManager.Subscribe<SetPageIndex>(SetPage);
		StateManager.Subscribe<SetCategory>(SetPage);
		StateManager.Subscribe<SetSpawnsetFilter>(SetPage);
		StateManager.Subscribe<SetAuthorFilter>(SetPage);
		StateManager.Subscribe<SetFeaturedOnly>(SetPage);
		StateManager.Subscribe<SetSorting>(SetPage);

		StateManager.Subscribe<SetLayout>(SetLayout);

		StateManager.Subscribe<SetCurrentPlayerId>(LoadAllLeaderboards);
		StateManager.Subscribe<ReloadAllCustomLeaderboards>(LoadAllLeaderboards);
	}

	private static void SetLayout()
	{
		if (StateManager.LayoutState.CurrentLayout != Root.Dependencies.CustomLeaderboardsRecorderMainLayout)
			return;

		LoadAllLeaderboards();
	}

	private static void LoadAllLeaderboards()
	{
		AsyncHandler.Run(p => StateManager.Dispatch(new CustomLeaderboardsLoaded(p)), () => FetchCustomLeaderboards.HandleAsync(UserCache.Model.PlayerId));
	}

	private void SetPage()
	{
		bool firstSelected = StateManager.LeaderboardListState.PageIndex == 0;
		bool lastSelected = StateManager.LeaderboardListState.PageIndex == StateManager.LeaderboardListState.GetTotalPages() - 1;
		_firstButton.IsDisabled = firstSelected;
		_prevButton.IsDisabled = firstSelected;
		_nextButton.IsDisabled = lastSelected;
		_lastButton.IsDisabled = lastSelected;

		_leaderboardListView.Set();
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.RectangleRenderer.Schedule(new(2, Bounds.Size.Y), new Vector2i<int>(Bounds.X1, Bounds.Center.Y) + scrollOffset, Depth - 5, Color.Gray(0.4f));

		Root.Game.MonoSpaceFontRenderer24.Schedule(new(1), scrollOffset + Bounds.TopLeft + new Vector2i<int>(4, 4), Depth + 2, Color.White, $"{StateManager.LeaderboardListState.Category} leaderboards", TextAlign.Left);
	}
}
