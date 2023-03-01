using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Types.Web;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListWrapper : AbstractComponent
{
	private const int _borderSize = 1;

	private readonly TooltipIconButton _firstButton;
	private readonly TooltipIconButton _prevButton;
	private readonly TooltipIconButton _nextButton;
	private readonly TooltipIconButton _lastButton;

	private readonly LeaderboardListView _leaderboardListView;

	public LeaderboardListWrapper(IBounds bounds)
		: base(bounds)
	{
		Dropdown categoryDropdown = new(bounds.CreateNested(88, 32, 96, 20), "Category", DropdownStyles.Default) { Depth = Depth + 1 };
		NestingContext.Add(categoryDropdown);

		CustomLeaderboardCategory[] categories = Enum.GetValues<CustomLeaderboardCategory>();
		for (int i = 0; i < categories.Length; i++)
		{
			CustomLeaderboardCategory category = categories[i];
			DropdownEntry dropdownEntry = new(categoryDropdown.Bounds.CreateNested(0, (i + 1) * 20, 96, 20), categoryDropdown, () => StateManager.Dispatch(new SetCategory(category)), category.ToString(), DropdownEntryStyles.Default)
			{
				IsActive = false,
				Depth = Depth + 100,
			};

			categoryDropdown.AddChild(dropdownEntry);
			NestingContext.Add(dropdownEntry);
		}

		_firstButton = new(bounds.CreateNested(4, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(0)), ButtonStyles.NavigationButton, WarpTextures.ArrowStart, "First", Color.HalfTransparentWhite, Color.White) { Depth = Depth + 100 };
		_prevButton = new(bounds.CreateNested(24, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.PageIndex - 1)), ButtonStyles.NavigationButton, WarpTextures.ArrowLeft, "Previous", Color.HalfTransparentWhite, Color.White) { Depth = Depth + 100 };
		_nextButton = new(bounds.CreateNested(44, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.PageIndex + 1)), ButtonStyles.NavigationButton, WarpTextures.ArrowRight, "Next", Color.HalfTransparentWhite, Color.White) { Depth = Depth + 100 };
		_lastButton = new(bounds.CreateNested(64, 32, 20, 20), () => StateManager.Dispatch(new SetPageIndex(StateManager.LeaderboardListState.MaxPageIndex)), ButtonStyles.NavigationButton, WarpTextures.ArrowEnd, "Last", Color.HalfTransparentWhite, Color.White) { Depth = Depth + 100 };

		NestingContext.Add(_firstButton);
		NestingContext.Add(_prevButton);
		NestingContext.Add(_nextButton);
		NestingContext.Add(_lastButton);

		const int offsetY = 64;
		_leaderboardListView = new(bounds.CreateNested(0, offsetY, bounds.Size.X, bounds.Size.Y - offsetY));
		NestingContext.Add(_leaderboardListView);

		StateManager.Subscribe<LoadLeaderboardList>(Load);
		StateManager.Subscribe<SetCategory>(Load);
		StateManager.Subscribe<SetPageIndex>(Load);
		StateManager.Subscribe<SetCurrentPlayerId>(Load);

		StateManager.Subscribe<PageLoaded>(SetPage);
	}

	private void SetPage()
	{
		bool firstSelected = StateManager.LeaderboardListState.PageIndex == 0;
		bool lastSelected = StateManager.LeaderboardListState.PageIndex == StateManager.LeaderboardListState.MaxPageIndex;
		_firstButton.IsDisabled = firstSelected;
		_prevButton.IsDisabled = firstSelected;
		_nextButton.IsDisabled = lastSelected;
		_lastButton.IsDisabled = lastSelected;

		_leaderboardListView.Set();
	}

	private void Load()
	{
		_leaderboardListView.Clear();

		_prevButton.IsDisabled = true;
		_nextButton.IsDisabled = true;

		AsyncHandler.Run(p => StateManager.Dispatch(new PageLoaded(p)), () => FetchCustomLeaderboards.HandleAsync(StateManager.LeaderboardListState.Category, StateManager.LeaderboardListState.PageIndex, StateManager.LeaderboardListState.PageSize, StateManager.RecordingState.CurrentPlayerId, false));
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center + scrollOffset, Depth, Color.Green);
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - new Vector2i<int>(_borderSize * 2), Bounds.Center, Depth + 1, Color.Gray(0.1f));

		Root.Game.MonoSpaceFontRenderer24.Schedule(new(1), scrollOffset + Bounds.TopLeft + new Vector2i<int>(4, 4), Depth + 2, Color.Yellow, $"{StateManager.LeaderboardListState.Category} leaderboards", TextAlign.Left);
	}
}
