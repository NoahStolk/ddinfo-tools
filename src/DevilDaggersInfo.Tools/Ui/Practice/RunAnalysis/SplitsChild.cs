using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

internal sealed class SplitsChild(FontService fontService)
{
	public void Render(PracticeStatsData statsData)
	{
		IReadOnlyList<SplitDataEntry> data = statsData.SplitsData.HomingSplitData;

		if (ImGui.BeginChild("Splits", new Vector2(192, 256)))
		{
			ImGuiExt.Title("Splits"u8, fontService.GoetheBold20);

			if (ImGui.BeginTable("LeaderboardTable", 3, ImGuiTableFlags.SizingStretchSame))
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
					ImGui.TextColored(textColor, Inline.Utf8(displayTimer));

					ImGui.TableNextColumn();
					ImGui.TextColored(textColor, homing.HasValue ? Inline.Utf8(homing.Value) : "-"u8);

					ImGui.TableNextColumn();
					int? delta = homing.HasValue && homingPrevious.HasValue ? homing.Value - homingPrevious.Value : null;
					Color color = delta switch
					{
						< 0 => Color.Red,
						> 0 => Color.Green,
						null => Color.Gray(0.5f),
						_ => Color.White,
					};
					ImGui.TextColored(color, delta.HasValue ? Inline.Utf8(delta.Value, "+0;-0;+0") : "-"u8);
				}

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
