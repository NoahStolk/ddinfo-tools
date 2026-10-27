using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using Hexa.NET.ImGui;
using Serilog;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;

internal sealed class LeaderboardChild(
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	PopupManager popupManager,
	GameMemoryService gameMemoryService,
	ILogger logger,
	UserSettings userSettings,
	UserCache userCache,
	SurvivalFileWatcher survivalFileWatcher)
{
	private readonly Func<CustomLeaderboardRankSorting, bool> _isTime = static rs => rs is CustomLeaderboardRankSorting.TimeAsc or CustomLeaderboardRankSorting.TimeDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isEnemiesAlive = static rs => rs is CustomLeaderboardRankSorting.EnemiesAliveAsc or CustomLeaderboardRankSorting.EnemiesAliveDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isEnemiesKilled = static rs => rs is CustomLeaderboardRankSorting.EnemiesKilledAsc or CustomLeaderboardRankSorting.EnemiesKilledDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isGemsCollected = static rs => rs is CustomLeaderboardRankSorting.GemsCollectedAsc or CustomLeaderboardRankSorting.GemsCollectedDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isGemsDespawned = static rs => rs is CustomLeaderboardRankSorting.GemsDespawnedAsc or CustomLeaderboardRankSorting.GemsDespawnedDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isGemsEaten = static rs => rs is CustomLeaderboardRankSorting.GemsEatenAsc or CustomLeaderboardRankSorting.GemsEatenDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isHomingStored = static rs => rs is CustomLeaderboardRankSorting.HomingStoredAsc or CustomLeaderboardRankSorting.HomingStoredDesc;
	private readonly Func<CustomLeaderboardRankSorting, bool> _isHomingEaten = static rs => rs is CustomLeaderboardRankSorting.HomingEatenAsc or CustomLeaderboardRankSorting.HomingEatenDesc;

	private GetCustomEntry? _selectedCustomEntry;

	private List<GetCustomEntry> _sortedEntries = [];

	public LeaderboardData? Data
	{
		get;
		set
		{
			field = value;
			_sortedEntries = value?.Leaderboard.SortedEntries ?? [];
			_selectedCustomEntry = null;
		}
	}

	public void Render()
	{
		if (ImGui.BeginChild("LeaderboardChild"))
		{
			if (Data == null)
				ImGui.Text("None selected");
			else
				RenderLeaderboard(Data);
		}

		ImGui.EndChild();
	}

	private void RenderLeaderboard(LeaderboardData data)
	{
		ImGui.Text(data.Leaderboard.SpawnsetName);

		if (ImGui.Button("Play", new Vector2(80, 20)))
			PlaySpawnset(data);

		if (survivalFileWatcher.SpawnsetName == data.Leaderboard.SpawnsetName)
		{
			ImGui.SameLine();
			ImGui.TextColored(new Vector4(0, 1, 0, 1), "Current spawnset");
		}

		if (_selectedCustomEntry != null)
		{
			ImGui.BeginDisabled(!_selectedCustomEntry.HasReplay);

			ImGui.SameLine();

			if (ImGui.Button(Inline.Utf8($"View {_selectedCustomEntry.PlayerName}'s replay in game")))
				WatchInGame(_selectedCustomEntry.Id);

			ImGui.SameLine();

			if (ImGui.Button(Inline.Utf8($"View {_selectedCustomEntry.PlayerName}'s replay in replay viewer")))
				WatchInReplayViewer(_selectedCustomEntry.Id);

			ImGui.EndDisabled();
		}

		if (ImGui.BeginChild("LeaderboardTableChild"))
			RenderTable(data.Leaderboard.RankSorting);

		ImGui.EndChild();
	}

	private void PlaySpawnset(LeaderboardData data)
	{
		AsyncHandler.Run(
			getSpawnsetResult =>
			{
				getSpawnsetResult.Match(
					getSpawnset => File.WriteAllBytes(userSettings.ModsSurvivalPath, getSpawnset.FileBytes),
					apiError => popupManager.ShowError("Could not fetch spawnset.", apiError));
			},
			() => FetchSpawnsetById.HandleAsync(data.SpawnsetId));
	}

	private unsafe void RenderTable(CustomLeaderboardRankSorting rankSorting)
	{
		const ImGuiTableFlags flags =
			ImGuiTableFlags.Resizable |
			ImGuiTableFlags.Reorderable |
			ImGuiTableFlags.Hideable |
			ImGuiTableFlags.Sortable |
			ImGuiTableFlags.SortMulti |
			ImGuiTableFlags.RowBg |
			ImGuiTableFlags.BordersV |
			ImGuiTableFlags.NoBordersInBody;

		ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(4, 1));
		if (ImGui.BeginTable("LeaderboardTable", 16, flags))
		{
			ImGui.TableSetupColumn("Rank", ImGuiTableColumnFlags.DefaultSort, 0, (int)LeaderboardSorting.Rank);
			ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.PlayerName);
			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.Time);
			ImGui.TableSetupColumn("Enemies alive", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.EnemiesAlive);
			ImGui.TableSetupColumn("Enemies killed", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.EnemiesKilled);
			ImGui.TableSetupColumn("Gems collected", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.GemsCollected);
			ImGui.TableSetupColumn("Gems despawned", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.GemsDespawned);
			ImGui.TableSetupColumn("Gems eaten", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.GemsEaten);
			ImGui.TableSetupColumn("Accuracy", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.Accuracy);
			ImGui.TableSetupColumn("Death type", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.DeathType);
			ImGui.TableSetupColumn("Homing stored", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.HomingStored);
			ImGui.TableSetupColumn("Homing eaten", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.HomingEaten);
			ImGui.TableSetupColumn("Level 2", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.LevelUpTime2);
			ImGui.TableSetupColumn("Level 3", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.LevelUpTime3);
			ImGui.TableSetupColumn("Level 4", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.LevelUpTime4);
			ImGui.TableSetupColumn("Submit date", ImGuiTableColumnFlags.None, 0, (int)LeaderboardSorting.SubmitDate);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.Handle != null && sortsSpecs.SpecsDirty)
			{
				Sort(sortsSpecs);
				sortsSpecs.SpecsDirty = false;
			}

			foreach (GetCustomEntry ce in _sortedEntries)
			{
				ImGui.TableNextRow();
				RenderCustomEntry(ce, rankSorting);
			}

			ImGui.EndTable();
		}

		ImGui.PopStyleVar();
	}

	private void RenderCustomEntry(GetCustomEntry ce, CustomLeaderboardRankSorting rankSorting)
	{
		ImGui.TableNextColumn();

		bool isCurrentPlayer = ce.PlayerId == userCache.Model.PlayerId;

		int headerAlpha = isCurrentPlayer ? 40 : 16;
		ImGui.PushStyleColor(ImGuiCol.Header, Colors.CustomLeaderboards.Primary with { A = (byte)headerAlpha });
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.CustomLeaderboards.Primary with { A = 64 });
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.CustomLeaderboards.Primary with { A = 96 });

		bool temp = true;
		if (ImGui.Selectable(Inline.Utf8(ce.Rank, "00"), ref temp, ImGuiSelectableFlags.SpanAllColumns))
			_selectedCustomEntry = ce;

		ImGui.PopStyleColor(3);

		ImGui.TableNextColumn();

		ImGui.TextColored(isCurrentPlayer ? new Vector4(0, 1, 0, 1) : Vector4.One, ce.PlayerName);
		ImGui.TableNextColumn();

		Color daggerColor = CustomLeaderboardDaggerUtils.GetColor(ce.CustomLeaderboardDagger);

		TextDaggerColored(Inline.Utf8(ce.TimeInSeconds, StringFormats.TimeFormat), _isTime);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Utf8(ce.EnemiesAlive), _isEnemiesAlive);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Utf8(ce.EnemiesKilled), _isEnemiesKilled);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Utf8(ce.GemsCollected), _isGemsCollected);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsDespawned.HasValue ? Inline.Utf8(ce.GemsDespawned.Value) : "-"u8, _isGemsDespawned);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsEaten.HasValue ? Inline.Utf8(ce.GemsEaten.Value) : "-"u8, _isGemsEaten);
		ImGui.TableNextColumn();

		ImGui.TextUnformatted(Inline.Utf8(GetAccuracy(ce), StringFormats.AccuracyFormat));
		ImGui.TableNextColumn();

		Death? death = Deaths.GetDeathByType(GameConstants.CurrentVersion, ce.DeathType);
		ImGui.TextColored(death?.Color ?? Vector4.One, death?.Name ?? "Unknown");
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Utf8(ce.HomingStored), _isHomingStored);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.HomingEaten.HasValue ? Inline.Utf8(ce.HomingEaten.Value) : "-"u8, _isHomingEaten);
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime2InSeconds == 0 ? "-"u8 : Inline.Utf8(ce.LevelUpTime2InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime3InSeconds == 0 ? "-"u8 : Inline.Utf8(ce.LevelUpTime3InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime4InSeconds == 0 ? "-"u8 : Inline.Utf8(ce.LevelUpTime4InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(Inline.Utf8(ce.SubmitDate, StringFormats.DateTimeFormat));
		ImGui.TableNextColumn();

		void TextDaggerColored(ReadOnlySpan<byte> text, Func<CustomLeaderboardRankSorting, bool> isRankSortingApplicable)
		{
			if (isRankSortingApplicable(rankSorting))
				ImGui.TextColored(daggerColor, text);
			else
				ImGui.Text(text);
		}
	}

	private void Sort(ImGuiTableSortSpecsPtr sortsSpecs)
	{
		LeaderboardSorting sorting = (LeaderboardSorting)sortsSpecs.Specs.ColumnUserID;
		bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;
		_sortedEntries =
		[
			.. sorting switch
			{
				LeaderboardSorting.Rank => Sort(sortAscending, ce => ce.Rank),
				LeaderboardSorting.PlayerName => Sort(sortAscending, ce => ce.PlayerName.ToLower()),
				LeaderboardSorting.Time => Sort(sortAscending, ce => ce.TimeInSeconds),
				LeaderboardSorting.EnemiesAlive => Sort(sortAscending, ce => ce.EnemiesAlive),
				LeaderboardSorting.EnemiesKilled => Sort(sortAscending, ce => ce.EnemiesKilled),
				LeaderboardSorting.GemsCollected => Sort(sortAscending, ce => ce.GemsCollected),
				LeaderboardSorting.GemsDespawned => Sort(sortAscending, ce => ce.GemsDespawned),
				LeaderboardSorting.GemsEaten => Sort(sortAscending, ce => ce.GemsEaten),
				LeaderboardSorting.Accuracy => Sort(sortAscending, GetAccuracy),
				LeaderboardSorting.DeathType => Sort(sortAscending, GetDeathSortingValue),
				LeaderboardSorting.HomingStored => Sort(sortAscending, ce => ce.HomingStored),
				LeaderboardSorting.HomingEaten => Sort(sortAscending, ce => ce.HomingEaten),
				LeaderboardSorting.LevelUpTime2 => Sort(sortAscending, ce => ce.LevelUpTime2InSeconds),
				LeaderboardSorting.LevelUpTime3 => Sort(sortAscending, ce => ce.LevelUpTime3InSeconds),
				LeaderboardSorting.LevelUpTime4 => Sort(sortAscending, ce => ce.LevelUpTime4InSeconds),
				LeaderboardSorting.SubmitDate => Sort(sortAscending, ce => ce.SubmitDate),
				_ => throw new UnreachableException($"Unknown sorting: {sorting}"),
			},
		];
	}

	private IOrderedEnumerable<GetCustomEntry> Sort<T>(bool sortAscending, Func<GetCustomEntry, T> keySelector)
	{
		return sortAscending ? _sortedEntries.OrderBy(keySelector) : _sortedEntries.OrderByDescending(keySelector);
	}

	private static string? GetDeathSortingValue(GetCustomEntry ce)
	{
		return Deaths.GetDeathByType(GameConstants.CurrentVersion, ce.DeathType)?.Name;
	}

	private static float GetAccuracy(GetCustomEntry ce)
	{
		return ce.DaggersFired == 0 ? 0 : ce.DaggersHit / (float)ce.DaggersFired;
	}

	private void WatchInGame(int id)
	{
		AsyncHandler.Run(Inject, () => FetchCustomEntryReplayById.HandleAsync(id));

		void Inject(ApiResult<GetCustomEntryReplayBuffer> getCustomEntryReplayBufferResult)
		{
			getCustomEntryReplayBufferResult.Match(
				getCustomEntryReplayBuffer => gameMemoryService.WriteReplayToMemory(getCustomEntryReplayBuffer.Data),
				apiError => popupManager.ShowError("Could not fetch replay.", apiError));
		}
	}

	private void WatchInReplayViewer(int id)
	{
		AsyncHandler.Run(BuildReplayScene, () => FetchCustomEntryReplayById.HandleAsync(id));

		void BuildReplayScene(ApiResult<GetCustomEntryReplayBuffer> getCustomEntryReplayBufferResult)
		{
			getCustomEntryReplayBufferResult.Match(
				getCustomEntryReplayBuffer =>
				{
					ReplayBinary<LocalReplayBinaryHeader> replayBinary;
					try
					{
						replayBinary = new ReplayBinary<LocalReplayBinaryHeader>(getCustomEntryReplayBuffer.Data);
					}
					catch (Exception ex)
					{
						logger.Error(ex, "Could not parse replay.");
						popupManager.ShowError("Could not parse replay.", ex);
						return;
					}

					customLeaderboards3DWindow.LoadReplay(replayBinary);
				},
				apiError => popupManager.ShowError("Could not fetch replay.", apiError));
		}
	}

	internal sealed record LeaderboardData(GetCustomLeaderboard Leaderboard, int SpawnsetId);
}
