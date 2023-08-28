using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.Api.App.Spawnsets;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.LeaderboardList;
using DevilDaggersInfo.App.User.Cache;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards.Leaderboard;

public static class LeaderboardChild
{
	private static GetCustomEntry? _selectedCustomEntry;

	private static List<GetCustomEntry> _sortedEntries = new();

	private static LeaderboardData? _data;
	public static LeaderboardData? Data
	{
		get => _data;
		set
		{
			_data = value;
			_sortedEntries = value?.Leaderboard.SortedEntries ?? new();
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

		ImGui.EndChild(); // End LeaderboardChild
	}

	private static void RenderLeaderboard(LeaderboardData data)
	{
		ImGui.Text(data.Leaderboard.SpawnsetName);

		if (ImGui.Button("Play", new(80, 20)))
		{
			AsyncHandler.Run(InstallSpawnset, () => FetchSpawnsetById.HandleAsync(data.SpawnsetId));
			void InstallSpawnset(GetSpawnset? spawnset)
			{
				if (spawnset == null)
				{
					Modals.ShowError("Could not fetch spawnset.");
					return;
				}

				File.WriteAllBytes(UserSettings.ModsSurvivalPath, spawnset.FileBytes);
			}
		}

		if (_selectedCustomEntry != null)
		{
			ImGui.BeginDisabled(!_selectedCustomEntry.HasReplay);

			ImGui.SameLine();

			if (ImGui.Button(UnsafeSpan.Get($"View {_selectedCustomEntry.PlayerName}'s replay in game")))
				WatchInGame(_selectedCustomEntry.Id);

			ImGui.SameLine();

			if (ImGui.Button(UnsafeSpan.Get($"View {_selectedCustomEntry.PlayerName}'s replay in replay viewer")))
				WatchInReplayViewer(_selectedCustomEntry.Id);

			ImGui.EndDisabled();
		}

		if (ImGui.BeginChild("LeaderboardTableChild"))
			RenderTable(data.Leaderboard.RankSorting);

		ImGui.EndChild(); // End LeaderboardTableChild
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

			ImGui.PopStyleVar();
			ImGui.EndTable();
		}
	}

	private static void RenderCustomEntry(GetCustomEntry ce, CustomLeaderboardRankSorting rankSorting)
	{
		ImGui.TableNextColumn();

		ImGui.PushStyleColor(ImGuiCol.Header, Colors.CustomLeaderboards.Primary with { A = 24 });
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.CustomLeaderboards.Primary with { A = 64 });
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.CustomLeaderboards.Primary with { A = 96 });
		bool temp = true;
		if (ImGui.Selectable(UnsafeSpan.Get(ce.Rank, "00"), ref temp, ImGuiSelectableFlags.SpanAllColumns))
			_selectedCustomEntry = ce;

		ImGui.PopStyleColor(3);

		ImGui.TableNextColumn();

		ImGui.TextColored(ce.PlayerId == UserCache.Model.PlayerId ? Color.Green : Color.White, ce.PlayerName);
		ImGui.TableNextColumn();

		Color daggerColor = CustomLeaderboardDaggerUtils.GetColor(ce.CustomLeaderboardDagger);

		TextDaggerColored(UnsafeSpan.Get(ce.TimeInSeconds, StringFormats.TimeFormat), static rs => rs is CustomLeaderboardRankSorting.TimeAsc or CustomLeaderboardRankSorting.TimeDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(UnsafeSpan.Get(ce.EnemiesAlive), static rs => rs is CustomLeaderboardRankSorting.EnemiesAliveAsc or CustomLeaderboardRankSorting.EnemiesAliveDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(UnsafeSpan.Get(ce.EnemiesKilled), static rs => rs is CustomLeaderboardRankSorting.EnemiesKilledAsc or CustomLeaderboardRankSorting.EnemiesKilledDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(UnsafeSpan.Get(ce.GemsCollected), static rs => rs is CustomLeaderboardRankSorting.GemsCollectedAsc or CustomLeaderboardRankSorting.GemsCollectedDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsDespawned.HasValue ? UnsafeSpan.Get(ce.GemsDespawned.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.GemsDespawnedAsc or CustomLeaderboardRankSorting.GemsDespawnedDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.GemsEaten.HasValue ? UnsafeSpan.Get(ce.GemsEaten.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.GemsEatenAsc or CustomLeaderboardRankSorting.GemsEatenDesc);
		ImGui.TableNextColumn();

		ImGui.TextUnformatted(UnsafeSpan.Get(GetAccuracy(ce), StringFormats.AccuracyFormat));
		ImGui.TableNextColumn();

		Death? death = Deaths.GetDeathByType(GameConstants.CurrentVersion, ce.DeathType);
		ImGui.TextColored(death?.Color.ToEngineColor() ?? Color.White, death?.Name ?? "Unknown");
		ImGui.TableNextColumn();

		TextDaggerColored(UnsafeSpan.Get(ce.HomingStored), static rs => rs is CustomLeaderboardRankSorting.HomingStoredAsc or CustomLeaderboardRankSorting.HomingStoredDesc);
		ImGui.TableNextColumn();

		TextDaggerColored(ce.HomingEaten.HasValue ? UnsafeSpan.Get(ce.HomingEaten.Value) : "-", static rs => rs is CustomLeaderboardRankSorting.HomingEatenAsc or CustomLeaderboardRankSorting.HomingEatenDesc);
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime2InSeconds == 0 ? "-" : UnsafeSpan.Get(ce.LevelUpTime2InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime3InSeconds == 0 ? "-" : UnsafeSpan.Get(ce.LevelUpTime3InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(ce.LevelUpTime4InSeconds == 0 ? "-" : UnsafeSpan.Get(ce.LevelUpTime4InSeconds, StringFormats.TimeFormat));
		ImGui.TableNextColumn();

		ImGui.Text(UnsafeSpan.Get(ce.SubmitDate, StringFormats.DateTimeFormat));
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
			return Deaths.GetDeathByLeaderboardType(GameConstants.CurrentVersion, ce.DeathType)?.Name;
		}
	}

	private static float GetAccuracy(GetCustomEntry ce)
	{
		return ce.DaggersFired == 0 ? 0 : ce.DaggersHit / (float)ce.DaggersFired;
	}

	private static void WatchInGame(int id)
	{
		AsyncHandler.Run(Inject, () => FetchCustomEntryReplayById.HandleAsync(id));

		void Inject(GetCustomEntryReplayBuffer? getCustomEntryReplayBuffer)
		{
			if (getCustomEntryReplayBuffer == null)
			{
				Modals.ShowError("Could not fetch replay.");
				return;
			}

			Root.GameMemoryService.WriteReplayToMemory(getCustomEntryReplayBuffer.Data);
		}
	}

	private static void WatchInReplayViewer(int id)
	{
		AsyncHandler.Run(BuildReplayScene, () => FetchCustomEntryReplayById.HandleAsync(id));

		void BuildReplayScene(GetCustomEntryReplayBuffer? getCustomEntryReplayBuffer)
		{
			if (getCustomEntryReplayBuffer == null)
			{
				Modals.ShowError("Could not fetch replay.");
				return;
			}

			ReplayBinary<LocalReplayBinaryHeader> replayBinary;
			try
			{
				replayBinary = new(getCustomEntryReplayBuffer.Data);
			}
			catch (Exception ex)
			{
				Root.Log.Error(ex, "Could not parse replay.");
				Modals.ShowError("Could not parse replay.");
				return;
			}

			CustomLeaderboards3DWindow.LoadReplay(replayBinary);
		}
	}

	public sealed record LeaderboardData(GetCustomLeaderboard Leaderboard, int SpawnsetId);
}
