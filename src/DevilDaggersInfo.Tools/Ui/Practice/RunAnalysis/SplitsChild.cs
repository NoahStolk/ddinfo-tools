using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

public static class SplitsChild
{
	public static void Render()
	{
		IReadOnlyList<SplitDataEntry> data = RunAnalysisWindow.StatsData.SplitsData.HomingSplitData;

		if (ImGui.BeginChild("Splits", new Vector2(192, 256)))
		{
			ImGuiExt.Title("Splits", Root.FontGoetheBold20);

			if (ImGui.BeginTable("LeaderboardTable", 3, ImGuiTableFlags.None))
			{
				ImGui.TableSetupColumn("Split", ImGuiTableColumnFlags.None, 40);
				ImGui.TableSetupColumn("Homing", ImGuiTableColumnFlags.None, 40);
				ImGui.TableSetupColumn("Delta", ImGuiTableColumnFlags.None, 40);
				ImGui.TableHeadersRow();

				for (int i = 0; i < data.Count; i++)
				{
					int displayTimer = data[i].DisplayTimer;
					int? homing = data[i].Homing;
					int? homingPrevious = data[i].HomingPrevious;

					Color textColor = homing.HasValue ? Color.White : Color.Gray(0.5f);
					uint backgroundColor = data[i].Kind switch
					{
						SplitDataEntryKind.Start => 0x2200ff00,
						SplitDataEntryKind.End => 0x220000ff,
						_ => 0x00000000,
					};
					ImGui.TableNextRow();

					ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, backgroundColor);

					ImGui.TableNextColumn();
					ImGui.TextColored(textColor, Inline.Span(displayTimer));

					ImGui.TableNextColumn();
					ImGui.TextColored(textColor, homing.HasValue ? Inline.Span(homing.Value) : "-");

					ImGui.TableNextColumn();
					int? delta = homing.HasValue && homingPrevious.HasValue ? homing.Value - homingPrevious.Value : null;
					Color color = delta switch
					{
						< 0 => Color.Red,
						> 0 => Color.Green,
						null => Color.Gray(0.5f),
						_ => Color.White,
					};
					ImGui.TextColored(color, delta.HasValue ? Inline.Span(delta.Value, "+0;-0;+0") : "-");

					ImGui.PopStyleColor(); // TODO: Remove?
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
