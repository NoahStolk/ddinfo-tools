using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Utils;
using DevilDaggersInfo.Common;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListEntry : LeaderboardListRow
{
	private readonly List<CriteriaIcon> _criteriaIcons = new();

	private readonly Label _name;
	private readonly Label _author;
	private readonly Label _score;
	private readonly Label _nextDagger;
	private readonly Label _rank;
	private readonly Label _players;
	private readonly Label _worldRecord;

	private GetCustomLeaderboardForOverview? _customLeaderboard;
	private bool _isHovering;

	public LeaderboardListEntry(IBounds bounds)
		: base(bounds)
	{
		int labelDepth = Depth + 100;
		_name = new(bounds.CreateNested(GridName.Index, 0, GridName.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth, RenderOverflow = false };
		_author = new(bounds.CreateNested(GridAuthor.Index, 0, GridAuthor.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth, RenderOverflow = false };
		_score = new(bounds.CreateNested(GridScore.Index, 0, GridScore.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };
		_nextDagger = new(bounds.CreateNested(GridNextDagger.Index, 0, GridNextDagger.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };
		_rank = new(bounds.CreateNested(GridRank.Index, 0, GridRank.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultRight) { Depth = labelDepth };
		_players = new(bounds.CreateNested(GridPlayers.Index, 0, GridPlayers.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultRight) { Depth = labelDepth };
		_worldRecord = new(bounds.CreateNested(GridWorldRecord.Index, 0, GridWorldRecord.Width, bounds.Size.Y), string.Empty, LabelStyles.DefaultLeft) { Depth = labelDepth };

		NestingContext.Add(_name);
		NestingContext.Add(_author);
		NestingContext.Add(_rank);
		NestingContext.Add(_players);
		NestingContext.Add(_score);
		NestingContext.Add(_nextDagger);
		NestingContext.Add(_worldRecord);
	}

	public void SetCustomLeaderboard(GetCustomLeaderboardForOverview? customLeaderboard)
	{
		_customLeaderboard = customLeaderboard;

		foreach (CriteriaIcon icon in _criteriaIcons)
			NestingContext.Remove(icon);

		_criteriaIcons.Clear();

		if (customLeaderboard == null)
		{
			_name.Text = string.Empty;
			_author.Text = string.Empty;
			_score.Text = string.Empty;
			_nextDagger.Text = string.Empty;
			_rank.Text = string.Empty;
			_players.Text = string.Empty;
			_worldRecord.Text = string.Empty;
			return;
		}

		int iconOffset = 0;
		foreach (GetCustomLeaderboardCriteria criteria in customLeaderboard.Criteria)
		{
			CriteriaIcon icon = new(Bounds.CreateNested(GridCriteria.Index + iconOffset, 0, 16, 16), criteria) { Depth = Depth + 1 };
			NestingContext.Add(icon);
			_criteriaIcons.Add(icon);

			iconOffset += 16;
		}

		_name.Text = customLeaderboard.SpawnsetName;
		_author.Text = customLeaderboard.SpawnsetAuthorName;
		_score.Text = customLeaderboard.SelectedPlayerStats?.Time.ToString(StringFormats.TimeFormat) ?? "-";
		_rank.Text = customLeaderboard.SelectedPlayerStats == null ? "-" : customLeaderboard.SelectedPlayerStats.Rank.ToString();
		_players.Text = customLeaderboard.PlayerCount.ToString();
		_worldRecord.Text = customLeaderboard.WorldRecord?.Time.ToString(StringFormats.TimeFormat) ?? "-";

		_score.LabelStyle = new(CustomLeaderboardDaggerUtils.GetColor(customLeaderboard.SelectedPlayerStats?.Dagger), TextAlign.Right, FontSize.H12, 4);
		_worldRecord.LabelStyle = new(CustomLeaderboardDaggerUtils.GetColor(customLeaderboard.WorldRecord?.Dagger), TextAlign.Right, FontSize.H12, 4);

		if (customLeaderboard.SelectedPlayerStats?.Dagger == CustomLeaderboardDagger.Leviathan)
		{
			_nextDagger.Text = "COMPLETED";
			_nextDagger.LabelStyle = new(CustomLeaderboardDaggerUtils.GetColor(CustomLeaderboardDagger.Leviathan), TextAlign.Right, FontSize.H12, 4);
		}
		else
		{
			_nextDagger.Text = customLeaderboard.SelectedPlayerStats?.NextDagger?.Time.ToString(StringFormats.TimeFormat) ?? "N/A";
			_nextDagger.LabelStyle = new(CustomLeaderboardDaggerUtils.GetColor(customLeaderboard.SelectedPlayerStats?.NextDagger?.Dagger), TextAlign.Right, FontSize.H12, 4);
		}
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		if (_customLeaderboard == null)
			return;

		_isHovering = MouseUiContext.Contains(scrollOffset, Bounds);
		if (!_isHovering || !Input.IsButtonPressed(MouseButton.Left))
			return;

		StateManager.Dispatch(new SetSelectedCustomLeaderboard(_customLeaderboard.Id, _customLeaderboard.SpawnsetId, _customLeaderboard.SpawnsetName));
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (_isHovering)
			Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth - 1, GlobalColors.EntrySelect);
	}
}
