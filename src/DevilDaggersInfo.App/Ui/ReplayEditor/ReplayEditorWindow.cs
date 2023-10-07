using DevilDaggersInfo.App.Ui.ReplayEditor.Events;
using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.App.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEditorWindow
{
	private static float _time;

	public static void Reset()
	{
		_time = 0;
		ReplayEventsChild.Reset();
		ReplayEntitiesChild.Reset();
	}

	public static void Update(float delta)
	{
		if (_time < ReplayState.Replay.Header.Time)
			_time += delta;

		ReplayEditor3DWindow.ArenaScene.CurrentTick = TimeUtils.TimeToTick(_time, 0);
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		if (ImGui.Begin("Replay Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
		{
			ImGui.PopStyleVar();

			ReplayEditorMenu.Render();

			ReplayFileInfo.Render();

			ImGui.SliderFloat("Time", ref _time, 0, ReplayState.Replay.Header.Time, "%.4f", ImGuiSliderFlags.NoInput);

			PlayerInputSnapshot snapshot = default;
			if (ReplayEditor3DWindow.ArenaScene.CurrentTick < ReplayEditor3DWindow.ArenaScene.ReplaySimulation?.InputSnapshots.Count)
				snapshot = ReplayEditor3DWindow.ArenaScene.ReplaySimulation.InputSnapshots[ReplayEditor3DWindow.ArenaScene.CurrentTick];

			Vector2 origin = ImGui.GetCursorScreenPos();
			ReplayInputs.Render(origin, snapshot);

			ImGui.SetCursorScreenPos(origin + new Vector2(0, 96));

			if (ImGui.BeginTabBar("Replay Editor Tabs"))
			{
				if (ImGui.BeginTabItem("Events"))
				{
					ReplayEventsChild.Render(ReplayState.Replay.EventsData, ReplayState.Replay.Header.StartTime);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities"))
				{
					ReplayEntitiesChild.Render(ReplayState.Replay.EventsData, ReplayState.Replay.Header.StartTime);
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}
		else
		{
			ImGui.PopStyleVar();
		}

		ImGui.End(); // End Replay Editor
	}
}
