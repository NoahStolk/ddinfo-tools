using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayShotgunSpeedChild
{
	private static List<TechSpeed.Entry> _entries = new();

	private static readonly Dictionary<(int TicksNone, int TicksHold), int> _counts = new();

	public static void Render(ReplayEventsData eventsData)
	{
		if (ImGui.Button("Get Speed"))
		{
			_entries = TechSpeed.ProcessReplay(eventsData);

			_counts.Clear();

			for (int i = 0; i < _entries.Count; i++)
			{
				TechSpeed.Entry entry = _entries[i];
				if (entry.DaggersFired is 10 or 20 or 40 or 60)
				{
					(int TicksNone, int TicksHold) key = (entry.TicksNone, entry.TicksHold);
					if (_counts.ContainsKey(key))
						_counts[key]++;
					else
						_counts.Add(key, 1);
				}
			}
		}

		if (ImGui.BeginChild("ReplayTechChild2", new(0, 0)))
		{
			ImGui.Text("Counts:");

			foreach ((int ticksNone, int ticksHold) in _counts.Keys.OrderBy(k => k.TicksNone).ThenBy(k => k.TicksHold))
			{
				int count = _counts[(ticksNone, ticksHold)];
				if (count is 1 or 2)
					continue;

				ImGui.Text(Inline.Span($"{ticksNone + ticksHold:00} ({ticksNone:00} {ticksHold:00}): {count}"));
			}
		}

		if (ImGui.BeginChild("ReplayTechChild", new(0, 0)))
		{
			if (!ImGui.BeginTable("ReplayTechTable", 5, ImGuiTableFlags.BordersInnerH))
				return;

			ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Shot state", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Tech state", ImGuiTableColumnFlags.WidthFixed, 256);
			ImGui.TableSetupColumn("Ticks up", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Ticks down", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableHeadersRow();

			for (int i = 0; i < _entries.Count; i++)
			{
				TechSpeed.Entry entry = _entries[i];

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span($"{TimeUtils.TickToTime(entry.Tick, 0):0.0000} ({entry.Tick})"));

				ImGui.TableNextColumn();
				bool shot = entry.DaggersFired is 10 or 20 or 40 or 60;
				ImGui.TextColored(shot ? new Vector4(1, 0.8f, 1, 1) : Color.Gray(0.4f), Inline.Span($"{entry.DaggersFired} daggers"));

				ImGui.TableNextColumn();
				bool usedTech = entry is { TicksNone: < 10, TicksHold: < 30 };
				ImGui.TextColored(!shot ? Color.Gray(0.4f) : usedTech ? Color.Orange : new Vector4(0.8f, 1, 0.8f, 1), Inline.Span(!shot ? "Just released LMB" : usedTech ? "Tech" : "Initiating tech / Single shot"));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(entry.TicksNone));

				ImGui.TableNextColumn();
				ImGui.Text(Inline.Span(entry.TicksHold));
			}

			ImGui.EndTable();
		}

		ImGui.EndChild(); // ReplayTechChild
	}
}
