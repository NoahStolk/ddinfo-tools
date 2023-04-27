using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Networking;
using DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.Base.User.Cache;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Utils;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Leaderboard;

public class LeaderboardEntry : AbstractComponent
{
	public const int Height = 16;

	private readonly GetCustomEntry _getCustomEntry;

	private readonly string _rank;
	private readonly string _time;
	private readonly string _enemiesAlive;
	private readonly string _enemiesKilled;
	private readonly string _gemsCollected;
	private readonly string _gemsDespawned;
	private readonly string _gemsEaten;
	private readonly string _accuracy;
	private readonly Death? _death;
	private readonly string _homingStored;
	private readonly string _homingEaten;
	private readonly string _level2;
	private readonly string _level3;
	private readonly string _level4;
	private readonly string _submitDate;

	public LeaderboardEntry(IBounds bounds, GetCustomEntry getCustomEntry)
		: base(bounds)
	{
		_getCustomEntry = getCustomEntry;

		_rank = _getCustomEntry.Rank.ToString("00");
		_time = _getCustomEntry.TimeInSeconds.ToString(StringFormats.TimeFormat);
		_enemiesAlive = _getCustomEntry.EnemiesAlive.ToString();
		_enemiesKilled = _getCustomEntry.EnemiesKilled.ToString();
		_gemsCollected = _getCustomEntry.GemsCollected.ToString();
		_gemsDespawned = _getCustomEntry.GemsDespawned?.ToString() ?? "-";
		_gemsEaten = _getCustomEntry.GemsEaten?.ToString() ?? "-";
		_accuracy = (_getCustomEntry.DaggersFired == 0 ? 0 : _getCustomEntry.DaggersHit / (float)_getCustomEntry.DaggersFired).ToString(StringFormats.AccuracyFormat);
		_death = Deaths.GetDeathByLeaderboardType(GameConstants.CurrentVersion, _getCustomEntry.DeathType);
		_homingStored = _getCustomEntry.HomingStored.ToString();
		_homingEaten = _getCustomEntry.HomingEaten?.ToString() ?? "-";
		_level2 = _getCustomEntry.LevelUpTime2InSeconds == 0 ? "-" : _getCustomEntry.LevelUpTime2InSeconds.ToString(StringFormats.TimeFormat);
		_level3 = _getCustomEntry.LevelUpTime3InSeconds == 0 ? "-" : _getCustomEntry.LevelUpTime3InSeconds.ToString(StringFormats.TimeFormat);
		_level4 = _getCustomEntry.LevelUpTime4InSeconds == 0 ? "-" : _getCustomEntry.LevelUpTime4InSeconds.ToString(StringFormats.TimeFormat);
		_submitDate = _getCustomEntry.SubmitDate.ToString(StringFormats.DateTimeFormat);

		TooltipIconButton watchInGame = new(Bounds.CreateNested(160, 0, 16, 16), WatchInGame, ButtonStyles.Borderless, Textures.IconEye, "Watch in-game", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { IsDisabled = !_getCustomEntry.HasReplay, Depth = Depth + 100 };
		TooltipIconButton watchInReplayViewer = new(Bounds.CreateNested(176, 0, 16, 16), WatchInReplayViewer, ButtonStyles.Borderless, Textures.IconEye, "Watch in replay viewer", TextAlign.Left, Color.HalfTransparentWhite, Color.White) { IsDisabled = !_getCustomEntry.HasReplay, Depth = Depth + 100 };

		NestingContext.Add(watchInGame);
		NestingContext.Add(watchInReplayViewer);
	}

	private void WatchInGame()
	{
		AsyncHandler.Run(Inject, () => FetchCustomEntryReplayById.HandleAsync(_getCustomEntry.Id));

		void Inject(GetCustomEntryReplayBuffer? getCustomEntryReplayBuffer)
		{
			if (getCustomEntryReplayBuffer == null)
			{
				Root.Dependencies.NativeDialogService.ReportError("Could not fetch replay.");
				return;
			}

			Root.Dependencies.GameMemoryService.WriteReplayToMemory(getCustomEntryReplayBuffer.Data);
		}
	}

	private void WatchInReplayViewer()
	{
		AsyncHandler.Run(BuildReplayScene, () => FetchCustomEntryReplayById.HandleAsync(_getCustomEntry.Id));

		void BuildReplayScene(GetCustomEntryReplayBuffer? getCustomEntryReplayBuffer)
		{
			if (getCustomEntryReplayBuffer == null)
			{
				Root.Dependencies.NativeDialogService.ReportError("Could not fetch replay.");
				return;
			}

			ReplayBinary<LocalReplayBinaryHeader> replayBinary;
			try
			{
				replayBinary = new(getCustomEntryReplayBuffer.Data);
			}
			catch (Exception ex)
			{
				Root.Dependencies.NativeDialogService.ReportError("Could not parse replay.", ex);
				return;
			}

			StateManager.Dispatch(new SetLayout(Root.Dependencies.CustomLeaderboardsRecorderReplayViewer3dLayout));
			StateManager.Dispatch(new BuildReplayScene(new[] { replayBinary }));
		}
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		bool isCurrentPlayer = _getCustomEntry.PlayerId == UserCache.Model.PlayerId;
		if (isCurrentPlayer)
		{
			Color color = new(0, 127, 0, 31);
			Root.Game.RectangleRenderer.Schedule(Bounds.Size, Bounds.Center + scrollOffset, Depth - 1, color);
		}

		Color textColor = isCurrentPlayer ? Color.Green : Color.White;

		Vector2i<int> position = Bounds.TopLeft + new Vector2i<int>(0, 4) + scrollOffset;
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[00], 0), Depth, textColor, _rank, TextAlign.Right);

		Vector2i<int> playerNamePosition = Bounds.TopLeft + new Vector2i<int>(0, 4) + new Vector2i<int>(LeaderboardWrapper.TableOffsets[01], 0);
		const int playerNameMaxWidth = 128;
		ScissorScheduler.PushScissor(Scissor.Create(new PixelBounds(playerNamePosition.X, playerNamePosition.Y, playerNameMaxWidth, 16), scrollOffset, ViewportState.Offset, ViewportState.Scale));
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[01], 0), Depth, textColor, _getCustomEntry.PlayerName, TextAlign.Left);
		ScissorScheduler.PopScissor();

		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[02], 0), Depth, CustomLeaderboardDaggerUtils.GetColor(_getCustomEntry.CustomLeaderboardDagger), _time, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[03], 0), Depth, Color.White, _enemiesAlive, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[04], 0), Depth, Color.White, _enemiesKilled, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[05], 0), Depth, Color.White, _gemsCollected, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[06], 0), Depth, Color.White, _gemsDespawned, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[07], 0), Depth, Color.White, _gemsEaten, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[08], 0), Depth, Color.White, _accuracy, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[09], 0), Depth, _death?.Color.ToEngineColor() ?? Color.White, _death?.Name ?? "-", TextAlign.Left);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[10], 0), Depth, Color.White, _homingStored, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[11], 0), Depth, Color.White, _homingEaten, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[12], 0), Depth, Color.White, _level2, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[13], 0), Depth, Color.White, _level3, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[14], 0), Depth, Color.White, _level4, TextAlign.Right);
		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), position + new Vector2i<int>(LeaderboardWrapper.TableOffsets[15], 0), Depth, Color.White, _submitDate, TextAlign.Right);
	}
}
