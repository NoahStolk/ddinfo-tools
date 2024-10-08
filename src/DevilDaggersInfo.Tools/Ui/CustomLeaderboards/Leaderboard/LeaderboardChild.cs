using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;

public static class LeaderboardChild
{
	private static GetCustomEntry? _selectedCustomEntry;

	private static List<GetCustomEntry> _sortedEntries = [];

	private static LeaderboardData? _data;
	public static LeaderboardData? Data
	{
		get => _data;
		set
		{
			_data = value;
			_sortedEntries = value?.Leaderboard.SortedEntries ?? [];
			_selectedCustomEntry = null;
		}
	}

	public static void Render()
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

	private static void RenderLeaderboard(LeaderboardData data)
	{
		ImGui.Text(data.Leaderboard.SpawnsetName);

		if (ImGui.Button("Play", new Vector2(80, 20)))
			PlaySpawnset(data);

		if (SurvivalFileWatcher.SpawnsetName == data.Leaderboard.SpawnsetName)
		{
			ImGui.SameLine();
			ImGui.TextColored(Color.Green, "Current spawnset");
		}

		if (_selectedCustomEntry != null)
		{
			ImGui.BeginDisabled(!_selectedCustomEntry.HasReplay);

			ImGui.SameLine();

			if (ImGui.Button(Inline.Span($"View {_selectedCustomEntry.PlayerName}'s replay in game")))
				WatchInGame(_selectedCustomEntry.Id);

			ImGui.SameLine();

			if (ImGui.Button(Inline.Span($"View {_selectedCustomEntry.PlayerName}'s replay in replay viewer")))
				WatchInReplayViewer(_selectedCustomEntry.Id);

			ImGui.EndDisabled();
		}

		if (ImGui.BeginChild("LeaderboardTableChild"))
			RenderTable(data.Leaderboard.RankSorting);

		ImGui.EndChild();
	}

	private static void PlaySpawnset(LeaderboardData data)
	{
		AsyncHandler.Run(
			getSpawnsetResult =>
			{
				getSpawnsetResult.Match(
					getSpawnset => File.WriteAllBytes(UserSettings.ModsSurvivalPath, getSpawnset.FileBytes),
					apiError => PopupManager.ShowError("Could not fetch spawnset.", apiError));
			},
			() => FetchSpawnsetById.HandleAsync(data.SpawnsetId));
	}

	private static unsafe void RenderTable(CustomLeaderboardRankSorting rankSorting)
	{
		const ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Sortable | ImGuiTableFlags.SortMulti | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody;

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
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
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

	private static void RenderCustomEntry(GetCustomEntry ce, CustomLeaderboardRankSorting rankSorting)
	{
		ImGui.TableNextColumn();

		ImGui.PushStyleColor(ImGuiCol.Header, Colors.CustomLeaderboards.Primary with { A = 24 });
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.CustomLeaderboards.Primary with { A = 64 });
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.CustomLeaderboards.Primary with { A = 96 });
		bool temp = true;
		if (ImGui.Selectable(Inline.Span(ce.Rank, "00"), ref temp, ImGuiSelectableFlags.SpanAllColumns))
			_selectedCustomEntry = ce;

		ImGui.PopStyleColor(3);

		ImGui.TableNextColumn();

		ImGui.TextColored(ce.PlayerId == UserCache.Model.PlayerId ? Color.Green : Color.White, ce.PlayerName);
		ImGui.TableNextColumn();

		Color daggerColor = CustomLeaderboardDaggerUtils.GetColor(ce.CustomLeaderboardDagger);

		TextDaggerColored(Inline.Span(ce.TimeInSeconds, StringFormats.TimeFormat), static rs => rs is CustomLeaderboardRankSorting.TimeAsc or CustomLeaderboardRankSorting.TimeDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Span(ce.EnemiesAlive), static rs => rs is CustomLeaderboardRankSorting.EnemiesAliveAsc or CustomLeaderboardRankSorting.EnemiesAliveDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Span(ce.EnemiesKilled), static rs => rs is CustomLeaderboardRankSorting.EnemiesKilledAsc or CustomLeaderboardRankSorting.EnemiesKilledDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Span(ce.GemsCollected), static rs => rs is CustomLeaderboardRankSorting.GemsCollectedAsc or CustomLeaderboardRankSorting.GemsCollectedDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsDespawned.HasValue ? Inline.Span(ce.GemsDespawned.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.GemsDespawnedAsc or CustomLeaderboardRankSorting.GemsDespawnedDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsEaten.HasValue ? Inline.Span(ce.GemsEaten.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.GemsEatenAsc or CustomLeaderboardRankSorting.GemsEatenDesc);
		ImGui.TableNextColumn();

		ImGui.TextUnformatted(Inline.Span(GetAccuracy(ce), StringFormats.AccuracyFormat));
		ImGui.TableNextColumn();

		Death? death = Deaths.GetDeathByType(GameConstants.CurrentVersion, ce.DeathType);
		ImGui.TextColored(death?.Color.ToEngineColor() ?? Color.White, death?.Name ?? "Unknown");
		ImGui.TableNextColumn();

		TextDaggerColored(Inline.Span(ce.HomingStored), static rs => rs is CustomLeaderboardRankSorting.HomingStoredAsc or CustomLeaderboardRankSorting.HomingStoredDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.HomingEaten.HasValue ? Inline.Span(ce.HomingEaten.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.HomingEatenAsc or CustomLeaderboardRankSorting.HomingEatenDesc);
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime2InSeconds == 0 ? "-" : Inline.Span(ce.LevelUpTime2InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime3InSeconds == 0 ? "-" : Inline.Span(ce.LevelUpTime3InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime4InSeconds == 0 ? "-" : Inline.Span(ce.LevelUpTime4InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(Inline.Span(ce.SubmitDate, StringFormats.DateTimeFormat));
		ImGui.TableNextColumn();

		void TextDaggerColored(ReadOnlySpan<char> text, Func<CustomLeaderboardRankSorting, bool> isRankSortingApplicable)
		{
			if (isRankSortingApplicable(rankSorting))
				ImGui.TextColored(daggerColor, text);
			else
				ImGui.Text(text);
		}
	}

	private static void Sort(ImGuiTableSortSpecsPtr sortsSpecs)
	{
		LeaderboardSorting sorting = (LeaderboardSorting)sortsSpecs.Specs.ColumnUserID;
		bool sortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;
		_sortedEntries = (sorting switch
		{
			LeaderboardSorting.Rank => sortAscending ? _sortedEntries.OrderBy(ce => ce.Rank) : _sortedEntries.OrderByDescending(ce => ce.Rank),
			LeaderboardSorting.PlayerName => sortAscending ? _sortedEntries.OrderBy(ce => ce.PlayerName.ToLower()) : _sortedEntries.OrderByDescending(ce => ce.PlayerName.ToLower()),
			LeaderboardSorting.Time => sortAscending ? _sortedEntries.OrderBy(ce => ce.TimeInSeconds) : _sortedEntries.OrderByDescending(ce => ce.TimeInSeconds),
			LeaderboardSorting.EnemiesAlive => sortAscending ? _sortedEntries.OrderBy(ce => ce.EnemiesAlive) : _sortedEntries.OrderByDescending(ce => ce.EnemiesAlive),
			LeaderboardSorting.EnemiesKilled => sortAscending ? _sortedEntries.OrderBy(ce => ce.EnemiesKilled) : _sortedEntries.OrderByDescending(ce => ce.EnemiesKilled),
			LeaderboardSorting.GemsCollected => sortAscending ? _sortedEntries.OrderBy(ce => ce.GemsCollected) : _sortedEntries.OrderByDescending(ce => ce.GemsCollected),
			LeaderboardSorting.GemsDespawned => sortAscending ? _sortedEntries.OrderBy(ce => ce.GemsDespawned) : _sortedEntries.OrderByDescending(ce => ce.GemsDespawned),
			LeaderboardSorting.GemsEaten => sortAscending ? _sortedEntries.OrderBy(ce => ce.GemsEaten) : _sortedEntries.OrderByDescending(ce => ce.GemsEaten),
			LeaderboardSorting.Accuracy => sortAscending ? _sortedEntries.OrderBy(GetAccuracy) : _sortedEntries.OrderByDescending(GetAccuracy),
			LeaderboardSorting.DeathType => sortAscending ? _sortedEntries.OrderBy(DeathSort) : _sortedEntries.OrderByDescending(DeathSort),
			LeaderboardSorting.HomingStored => sortAscending ? _sortedEntries.OrderBy(ce => ce.HomingStored) : _sortedEntries.OrderByDescending(ce => ce.HomingStored),
			LeaderboardSorting.HomingEaten => sortAscending ? _sortedEntries.OrderBy(ce => ce.HomingEaten) : _sortedEntries.OrderByDescending(ce => ce.HomingEaten),
			LeaderboardSorting.LevelUpTime2 => sortAscending ? _sortedEntries.OrderBy(ce => ce.LevelUpTime2InSeconds) : _sortedEntries.OrderByDescending(ce => ce.LevelUpTime2InSeconds),
			LeaderboardSorting.LevelUpTime3 => sortAscending ? _sortedEntries.OrderBy(ce => ce.LevelUpTime3InSeconds) : _sortedEntries.OrderByDescending(ce => ce.LevelUpTime3InSeconds),
			LeaderboardSorting.LevelUpTime4 => sortAscending ? _sortedEntries.OrderBy(ce => ce.LevelUpTime4InSeconds) : _sortedEntries.OrderByDescending(ce => ce.LevelUpTime4InSeconds),
			LeaderboardSorting.SubmitDate => sortAscending ? _sortedEntries.OrderBy(ce => ce.SubmitDate) : _sortedEntries.OrderByDescending(ce => ce.SubmitDate),
			_ => throw new UnreachableException(),
		}).ToList();

		static string? DeathSort(GetCustomEntry ce)
		{
			return Deaths.GetDeathByType(GameConstants.CurrentVersion, ce.DeathType)?.Name;
		}
	}

	private static float GetAccuracy(GetCustomEntry ce)
	{
		return ce.DaggersFired == 0 ? 0 : ce.DaggersHit / (float)ce.DaggersFired;
	}

	private static void WatchInGame(int id)
	{
		AsyncHandler.Run(Inject, () => FetchCustomEntryReplayById.HandleAsync(id));

		void Inject(ApiResult<GetCustomEntryReplayBuffer> getCustomEntryReplayBufferResult)
		{
			getCustomEntryReplayBufferResult.Match(
				getCustomEntryReplayBuffer => Root.GameMemoryService.WriteReplayToMemory(getCustomEntryReplayBuffer.Data),
				apiError => PopupManager.ShowError("Could not fetch replay.", apiError));
		}
	}

	private static void WatchInReplayViewer(int id)
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
						Root.Log.Error(ex, "Could not parse replay.");
						PopupManager.ShowError("Could not parse replay.", ex);
						return;
					}

					CustomLeaderboards3DWindow.LoadReplay(replayBinary);
				},
				apiError => PopupManager.ShowError("Could not fetch replay.", apiError));
		}
	}

	public sealed record LeaderboardData(GetCustomLeaderboard Leaderboard, int SpawnsetId);
}
