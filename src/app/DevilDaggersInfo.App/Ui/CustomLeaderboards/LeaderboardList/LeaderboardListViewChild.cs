using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.CriteriaExpression;
using DevilDaggersInfo.Core.CriteriaExpression.Extensions;
using DevilDaggersInfo.Core.CriteriaExpression.Parts;
using ImGuiNET;
using System.Numerics;
using System.Text;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards.LeaderboardList;

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

				ImGui.Text($"Page {page} of {totalPages} ({start}-{end} of {totalEntries})");

				RenderTable();
			}
		}

		ImGui.EndChild(); // End LeaderboardList
	}

	private static unsafe void RenderTable()
	{
		const ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Sortable | ImGuiTableFlags.SortMulti | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersV | ImGuiTableFlags.NoBordersInBody;

		ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(4, 1));
		if (ImGui.BeginTable("LeaderboardListTable", 8, flags))
		{
			ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort, 0, (int)LeaderboardListSorting.Name);
			ImGui.TableSetupColumn("Author", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Author);
			ImGui.TableSetupColumn("Criteria", ImGuiTableColumnFlags.NoSort);
			ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Score);
			ImGui.TableSetupColumn("Next dagger", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.NextDagger);
			ImGui.TableSetupColumn("Rank", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Rank);
			ImGui.TableSetupColumn("Players", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.Players);
			ImGui.TableSetupColumn("World record", ImGuiTableColumnFlags.None, 0, (int)LeaderboardListSorting.WorldRecord);
			ImGui.TableHeadersRow();

			ImGuiTableSortSpecsPtr sortsSpecs = ImGui.TableGetSortSpecs();
			if (sortsSpecs.NativePtr != (void*)0 && sortsSpecs.SpecsDirty)
			{
				LeaderboardListChild.Sorting = (LeaderboardListSorting)sortsSpecs.Specs.ColumnUserID;
				LeaderboardListChild.SortAscending = sortsSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending;
				LeaderboardListChild.UpdatePagedCustomLeaderboards();
				sortsSpecs.SpecsDirty = false;
			}

			foreach (GetCustomLeaderboardForOverview clOverview in LeaderboardListChild.PagedCustomLeaderboards)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				ImGui.PushStyleColor(ImGuiCol.Header, Colors.CustomLeaderboards.Primary with { A = 24 });
				ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Colors.CustomLeaderboards.Primary with { A = 64 });
				ImGui.PushStyleColor(ImGuiCol.HeaderActive, Colors.CustomLeaderboards.Primary with { A = 96 });
				bool temp = true;
				if (ImGui.Selectable(clOverview.SpawnsetName, ref temp, ImGuiSelectableFlags.SpanAllColumns, new(0, 16)))
				{
					AsyncHandler.Run(
						cl =>
						{
							if (cl == null)
							{
								Modals.ShowError("Could not fetch custom leaderboard.");
								LeaderboardChild.Data = null;
							}
							else
							{
								LeaderboardChild.Data = new(cl, clOverview.SpawnsetId);
							}
						},
						() => FetchCustomLeaderboardById.HandleAsync(clOverview.Id));
				}

				ImGui.PopStyleColor(3);

				ImGui.TableNextColumn();

				ImGui.Text(clOverview.SpawnsetAuthorName);
				ImGui.TableNextColumn();

				for (int i = 0; i < clOverview.Criteria.Count; i++)
				{
					GetCustomLeaderboardCriteria criteria = clOverview.Criteria[i];
					ImGuiImage.Image(criteria.Type.GetTexture().Handle, new(16), criteria.Type.GetColor());
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

				ImGui.TextColored(CustomLeaderboardDaggerUtils.GetColor(clOverview.SelectedPlayerStats?.Dagger), clOverview.SelectedPlayerStats == null ? "-" : UnsafeSpan.Get(clOverview.SelectedPlayerStats.HighscoreValue, valueFormat));
				ImGui.TableNextColumn();

				bool completed = clOverview.SelectedPlayerStats?.Dagger == CustomLeaderboardDagger.Leviathan;
				Color color = CustomLeaderboardDaggerUtils.GetColor(completed ? CustomLeaderboardDagger.Leviathan : clOverview.SelectedPlayerStats?.NextDagger?.Dagger);
				ImGui.TextColored(color, completed ? "COMPLETED" : clOverview.SelectedPlayerStats?.NextDagger == null ? "N/A" : UnsafeSpan.Get(clOverview.SelectedPlayerStats.NextDagger.DaggerValue, valueFormat));
				ImGui.TableNextColumn();

				ImGui.Text(clOverview.SelectedPlayerStats == null ? "-" : UnsafeSpan.Get(clOverview.SelectedPlayerStats.Rank));
				ImGui.TableNextColumn();

				ImGui.Text(UnsafeSpan.Get(clOverview.PlayerCount));
				ImGui.TableNextColumn();

				ImGui.TextColored(CustomLeaderboardDaggerUtils.GetColor(clOverview.WorldRecord?.Dagger), clOverview.WorldRecord == null ? "-" : UnsafeSpan.Get(clOverview.WorldRecord.WorldRecordValue, valueFormat));
				ImGui.TableNextColumn();
			}

			ImGui.PopStyleVar();
			ImGui.EndTable();
		}
	}

	private static string GetText(GetCustomLeaderboardCriteria criteria)
	{
		if (!Expression.TryParse(criteria.Expression, out Expression? criteriaExpression))
		{
			// TODO: Log warning.
			return string.Empty;
		}

		DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType criteriaType = criteria.Type.ToCore();
		DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator @operator = criteria.Operator.ToCore();

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
