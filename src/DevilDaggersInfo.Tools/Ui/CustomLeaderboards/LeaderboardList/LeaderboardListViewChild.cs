using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.CriteriaExpression;
using DevilDaggersInfo.Core.CriteriaExpression.Extensions;
using DevilDaggersInfo.Core.CriteriaExpression.Parts;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using ImGuiNET;
using System.Numerics;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;

public static class LeaderboardListViewChild
{
	public static void Render()
	{
		if (ImGui.BeginChild("LeaderboardList"))
		{
			if (LeaderboardListChild.IsLoading)
			{
				ImGui.TextColored(Color.Yellow, "Loading...");
			}
			else
			{
				int page = LeaderboardListChild.PageIndex + 1;
				int totalPages = LeaderboardListChild.TotalPages;
				int totalEntries = LeaderboardListChild.TotalEntries;

				int start = LeaderboardListChild.PageIndex * LeaderboardListChild.PageSize + 1;
				int end = Math.Min((LeaderboardListChild.PageIndex + 1) * LeaderboardListChild.PageSize, totalEntries);

				ImGui.Text(Inline.Span($"Page {page} of {totalPages} ({start}-{end} of {totalEntries})"));

				RenderTable();
			}
		}

		ImGui.EndChild(); // End LeaderboardList
	}

	private static unsafe void RenderTable()
	{
		const ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Sortable | ImGuiTableFlags.SortMulti | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody;

		ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(4, 1));
		if (ImGui.BeginTable("LeaderboardListTable", 10, flags))
		{
			ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort, 0, (int)LeaderboardListSorting.Name);
			ImGui.TableSetupColumn("Author", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Author);
			ImGui.TableSetupColumn("Criteria", ImGuiTableColumnFlags.NoSort);
			ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Score);
			ImGui.TableSetupColumn("Next dagger", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.NextDagger);
			ImGui.TableSetupColumn("Rank", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Rank);
			ImGui.TableSetupColumn("Players", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Players);
			ImGui.TableSetupColumn("World record", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.WorldRecord);
			ImGui.TableSetupColumn("Date created", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.DateCreated);
			ImGui.TableSetupColumn("Date last played", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.DateLastPlayed);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				LeaderboardListChild.Sorting = (LeaderboardListSorting)sortsSpecs.Specs.ColumnUserID;
				LeaderboardListChild.SortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;
				LeaderboardListChild.UpdatePagedCustomLeaderboards();
				sortsSpecs.SpecsDirty = false;
			}

			for (int i = 0; i < LeaderboardListChild.PagedCustomLeaderboards.Count; i++)
			{
				GetCustomLeaderboardForOverview clOverview = LeaderboardListChild.PagedCustomLeaderboards[i];
				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				ImGui.PushStyleColor(ImGuiCol.Header, Colors.CustomLeaderboards.Primary with { A = 24 });
				ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.CustomLeaderboards.Primary with { A = 64 });
				ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.CustomLeaderboards.Primary with { A = 96 });
				bool temp = true;
				if (ImGui.Selectable(clOverview.SpawnsetName, ref temp, ImGuiSelectableFlags.SpanAllColumns, new Vector2(0, 16)))
					SelectLeaderboard(clOverview);

				ImGui.PopStyleColor(3);

				ImGui.TableNextColumn();

				ImGui.Text(clOverview.SpawnsetAuthorName);
				ImGui.TableNextColumn();

				for (int j = 0; j < clOverview.Criteria.Count; j++)
				{
					GetCustomLeaderboardCriteria criteria = clOverview.Criteria[j];
					ImGuiImage.Image(criteria.Type.GetTexture().Id, new Vector2(16), criteria.Type.GetColor());
					if (ImGui.IsItemHovered())
					{
						// TODO: May need to improve performance here by caching the text, or perhaps return the text from the API.
						ImGui.SetTooltip(GetText(criteria));
					}

					ImGui.SameLine();
				}

				ImGui.TableNextColumn();

				string valueFormat = clOverview.RankSorting switch
				{
					CustomLeaderboardRankSorting.TimeAsc or CustomLeaderboardRankSorting.TimeDesc => StringFormats.TimeFormat,
					_ => "0",
				};

				ImGui.TextColored(CustomLeaderboardDaggerUtils.GetColor(clOverview.SelectedPlayerStats?.Dagger), clOverview.SelectedPlayerStats == null ? "-" : Inline.Span(clOverview.SelectedPlayerStats.HighscoreValue, valueFormat));
				ImGui.TableNextColumn();

				bool completed = clOverview.SelectedPlayerStats?.Dagger == CustomLeaderboardDagger.Leviathan;
				Color color = CustomLeaderboardDaggerUtils.GetColor(completed ? CustomLeaderboardDagger.Leviathan : clOverview.SelectedPlayerStats?.NextDagger?.Dagger);
				ImGui.TextColored(color, completed ? "COMPLETED" : clOverview.SelectedPlayerStats?.NextDagger == null ? "N/A" : Inline.Span(clOverview.SelectedPlayerStats.NextDagger.DaggerValue, valueFormat));
				ImGui.TableNextColumn();

				ImGui.Text(clOverview.SelectedPlayerStats == null ? "-" : Inline.Span(clOverview.SelectedPlayerStats.Rank));
				ImGui.TableNextColumn();

				ImGui.Text(Inline.Span(clOverview.PlayerCount));
				ImGui.TableNextColumn();

				ImGui.TextColored(CustomLeaderboardDaggerUtils.GetColor(clOverview.WorldRecord?.Dagger), clOverview.WorldRecord == null ? "-" : Inline.Span(clOverview.WorldRecord.WorldRecordValue, valueFormat));
				ImGui.TableNextColumn();

				ImGui.Text(clOverview.DateCreated.ToString(StringFormats.DateTimeFormat));
				ImGui.TableNextColumn();

				ImGui.Text(clOverview.DateLastPlayed?.ToString(StringFormats.DateTimeFormat) ?? "Never");
				ImGui.TableNextColumn();
			}

			ImGui.EndTable();
		}

		ImGui.PopStyleVar();
	}

	private static void SelectLeaderboard(GetCustomLeaderboardForOverview clOverview)
	{
		AsyncHandler.Run(
			getLeaderboardResult =>
			{
				getLeaderboardResult.Match(
					onSuccess: getLeaderboard => LeaderboardChild.Data = new LeaderboardChild.LeaderboardData(getLeaderboard, clOverview.SpawnsetId),
					onError: apiError =>
					{
						PopupManager.ShowError("Could not fetch custom leaderboard.", apiError);
						LeaderboardChild.Data = null;
					});
			},
			() => FetchCustomLeaderboardById.HandleAsync(clOverview.Id));
	}

	private static string GetText(GetCustomLeaderboardCriteria criteria)
	{
		if (!Expression.TryParse(criteria.Expression, out Expression? criteriaExpression))
		{
			// TODO: Log warning.
			return string.Empty;
		}

		Core.CriteriaExpression.CustomLeaderboardCriteriaType criteriaType = criteria.Type.ToCore();
		Core.CriteriaExpression.CustomLeaderboardCriteriaOperator @operator = criteria.Operator.ToCore();

		StringBuilder sb = new();
		sb.Append(criteriaType.Display());
		sb.Append(' ');
		sb.Append(@operator.ShortString());
		sb.Append(' ');

		foreach (IExpressionPart expressionPart in criteriaExpression.Parts)
		{
			switch (expressionPart)
			{
				case ExpressionOperator op:
					sb.Append(op);
					break;
				case ExpressionTarget target:
					sb.Append(target);
					break;
				case ExpressionValue value:
					sb.Append(value.ToDisplayString(criteriaType));
					break;
			}

			sb.Append(' ');
		}

		return sb.ToString().Trim();
	}
}
