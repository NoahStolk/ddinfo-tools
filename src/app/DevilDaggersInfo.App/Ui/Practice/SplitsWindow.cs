using DevilDaggersInfo.App.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice;

public static class SplitsWindow
{
	private static float _recordingTimer;

	private static readonly List<int> _homingStored = new();

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

	public static void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		_homingStored.Clear();

		byte[] statsBuffer = Root.GameMemoryService.GetStatsBuffer();
		using MemoryStream ms = new(statsBuffer);
		using BinaryReader br = new(ms);
		for (int i = 0; i < Root.GameMemoryService.MainBlock.StatsCount; i++)
		{
			br.BaseStream.Seek(sizeof(int) * 6, SeekOrigin.Current); // Skip 6 int stats.
			_homingStored.Add(br.ReadInt32());
			br.BaseStream.Seek(sizeof(int) * 4, SeekOrigin.Current); // Skip 4 int stats.
			br.BaseStream.Seek(sizeof(ushort) * 17 * 2, SeekOrigin.Current); // Skip 17 ushort stats (two for each enemy).
		}
	}

	public static void Render()
	{
		Span<int?> relevantHomingValues = stackalloc int?[_splitData.Count];
		for (int i = 0; i < _splitData.Count; i++)
		{
			KeyValuePair<int, int> splitEntry = _splitData.ElementAt(i);
			int actualIndex = Math.Max(0, splitEntry.Value - 350); // TEMP
			bool hasValue = _homingStored.Count > actualIndex;
			relevantHomingValues[i] = hasValue ? _homingStored[actualIndex] : null;
		}

		ImGui.SetNextWindowSize(new(512, 320), ImGuiCond.Always);
		ImGui.Begin("Splits", ImGuiWindowFlags.NoResize);

		if (ImGui.BeginTable("LeaderboardTable", 4, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Name");
			ImGui.TableSetupColumn("Seconds");
			ImGui.TableSetupColumn("Homing");
			ImGui.TableSetupColumn("Split");
			ImGui.TableHeadersRow();

			for (int i = 0; i < _splitData.Count; i++)
			{
				KeyValuePair<int, int> splitEntry = _splitData.ElementAt(i);
				int? homing = relevantHomingValues[i];
				int previousHoming = i > 0 ? relevantHomingValues[i - 1] ?? 0 : 0;

				ImGui.BeginDisabled(!homing.HasValue);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text(splitEntry.Key.ToString());

				ImGui.TableNextColumn();
				ImGui.Text(splitEntry.Value.ToString());

				ImGui.TableNextColumn();
				ImGui.Text(homing.HasValue ? homing.Value.ToString() : "N/A");

				ImGui.TableNextColumn();
				if (homing.HasValue)
				{
					int delta = homing.Value - previousHoming;
					Color color = delta switch
					{
						< 0 => Color.Red,
						> 0 => Color.Green,
						_ => Color.White,
					};
					ImGui.TextColored(color, delta.ToString("+0;-0;+0"));
				}
				else
				{
					ImGui.Text("N/A");
				}

				ImGui.EndDisabled();
			}

			ImGui.EndTable();
		}

		ImGui.End();
	}
}
