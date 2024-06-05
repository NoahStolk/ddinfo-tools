using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.EditorFileState;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineActionsChild
{
	private static int _tickCount = 5;

	public static void Render()
	{
		if (ImGui.BeginChild("ActionsChild", new Vector2(320, 96)))
		{
			ImGui.PushItemWidth(128);
			ImGui.InputInt("Amount of ticks to insert", ref _tickCount);
			ImGui.PopItemWidth();

			if (ImGui.Button("Insert empty data"))
			{
				for (int i = 0; i < _tickCount; i++)
					FileStates.Replay.Object.InputsEvents.Add(InputsEventData.CreateDefault());

				TimelineCache.Clear();
			}
		}

		ImGui.EndChild(); // End ActionsChild
	}
}
