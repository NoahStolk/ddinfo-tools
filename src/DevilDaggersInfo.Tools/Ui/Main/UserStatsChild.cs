using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.User.Stats;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Main;

internal sealed class UserStatsChild(UserStats userStats)
{
	public void Render()
	{
		RenderContents();
	}

	private void RenderContents()
	{
		if (userStats.PlayerStats == null)
		{
			ImGui.TextColored(new Vector4(1, 1, 0, 1), "Not fetched");
			return;
		}

		FetchPlayerStats.EntryResponse stats = userStats.PlayerStats;

		const float tableWidth = 512;
		const float cellRightWidth = 256;

		if (ImGui.BeginTable("Info", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, cellRightWidth);

			NextColumnText("Last fetched"u8, Inline.Utf8(new TimeAgo(userStats.LastFetchedUtc)));
			NextColumnText("Player ID"u8, Inline.Utf8(stats.Id));
			NextColumnText("Username"u8, stats.Username);

			ImGui.EndTable();
		}

		ImGui.SeparatorText("Highscore");
		if (ImGui.BeginTable("Highscore", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, cellRightWidth);

			NextColumnText("Rank"u8, Inline.Utf8(stats.Rank, "N0"));
			NextColumnText("Time"u8, Inline.Utf8(stats.Time.Seconds, "N4"));
			NextColumnText("Kills"u8, Inline.Utf8(stats.Kills, "N0"));
			NextColumnText("Gems"u8, Inline.Utf8(stats.Gems, "N0"));
			NextColumnText("Death"u8, DeathTypeUtils.GetDeathTypeSpan(stats.DeathType));
			NextColumnText("Accuracy"u8, FormatAccuracy(stats.DaggersFired, stats.DaggersHit));

			ImGui.EndTable();
		}

		ImGui.SeparatorText("Total stats");
		if (ImGui.BeginTable("TotalStats", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
		{
			ImGui.TableSetupColumn("##left", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("##right", ImGuiTableColumnFlags.WidthFixed, cellRightWidth);

			NextColumnText("Total Time"u8, Inline.Utf8(stats.TimeTotal.Seconds, "N4"));
			NextColumnText("Total Kills"u8, Inline.Utf8(stats.KillsTotal, "N0"));
			NextColumnText("Total Gems"u8, Inline.Utf8(stats.GemsTotal, "N0"));
			NextColumnText("Total Deaths"u8, Inline.Utf8(stats.DeathsTotal, "N0"));
			NextColumnText("Total Accuracy"u8, FormatAccuracy(stats.DaggersFiredTotal, stats.DaggersHitTotal));

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<byte> FormatAccuracy<T>(T fired, T hit)
		where T : IBinaryInteger<T>
	{
		float value = AccuracyUtils.GetAccuracy(fired, hit);
		return Inline.Utf8($"{value:0.00%} ({hit:N0} / {fired:N0})");
	}

	private static void NextColumnText(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		ImGui.TableNextColumn();
		ImGui.Text(left);

		ImGui.TableNextColumn();
		ImGui.TextUnformatted(right);
	}

	private static void NextColumnText(ReadOnlySpan<byte> left, string right)
	{
		ImGui.TableNextColumn();
		ImGui.Text(left);

		ImGui.TableNextColumn();
		ImGui.TextUnformatted(right);
	}
}
