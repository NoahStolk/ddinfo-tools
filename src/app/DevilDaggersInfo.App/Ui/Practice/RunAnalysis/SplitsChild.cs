using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis;

public static class SplitsChild
{
	private static readonly IReadOnlyDictionary<int, int> _splitData = new Dictionary<int, int>
	{
		[350] = 366,
		[700] = 709,
		[800] = 800,
		[880] = 875,
		[930] = 942,
		[1000] = 996,
		[1040] = 1047,
		[1080] = 1091,
		[1130] = 1133,
		[1160] = 1170,
	};
	private static readonly int?[] _relevantHomingValues = new int?[_splitData.Count];

	public static void Render()
	{
		if (ImGui.BeginChild("Splits", new(160, 192)))
		{
			for (int i = 0; i < _splitData.Count; i++)
			{
				KeyValuePair<int, int> splitEntry = _splitData.ElementAt(i); // TODO: This allocates heap memory.
				int actualIndex = Math.Max(0, splitEntry.Value - 350); // TEMP
				bool hasValue = RunAnalysisWindow.StatsData.Statistics.Count > actualIndex;
				_relevantHomingValues[i] = hasValue ? RunAnalysisWindow.StatsData.Statistics[actualIndex].HomingStored : null;
			}

			if (ImGui.BeginTable("LeaderboardTable", 3, ImGuiTableFlags.None))
			{
				ImGui.TableSetupColumn("Split", ImGuiTableColumnFlags.None, 24);
				ImGui.TableSetupColumn("Homing", ImGuiTableColumnFlags.None, 32);
				ImGui.TableSetupColumn("Delta", ImGuiTableColumnFlags.None, 32);
				ImGui.TableHeadersRow();

				for (int i = 0; i < _splitData.Count; i++)
				{
					KeyValuePair<int, int> splitEntry = _splitData.ElementAt(i); // TODO: This allocates heap memory.
					int? homing = _relevantHomingValues[i];
					int? previousHoming = i > 0 ? _relevantHomingValues[i - 1] : null;

					if (!homing.HasValue || !previousHoming.HasValue)
						continue;

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(splitEntry.Key.ToString());

					ImGui.TableNextColumn();
					ImGui.Text(homing.Value.ToString());

					ImGui.TableNextColumn();
					int delta = homing.Value - previousHoming.Value;
					Color color = delta switch
					{
						< 0 => Color.Red,
						> 0 => Color.Green,
						_ => Color.White,
					};
					ImGui.TextColored(color, delta.ToString("+0;-0;+0"));
				}

				ImGui.EndTable();
			}

			ImGui.EndChild();
		}
	}
}
