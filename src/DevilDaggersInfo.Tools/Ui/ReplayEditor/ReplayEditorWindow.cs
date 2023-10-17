using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayEditorWindow
{
	public static void Reset()
	{
		ReplayEditor3DWindow.Reset();
		ReplayEventsChild.Reset();
		ReplayEntitiesChild.Reset();
	}

	public static void Update(float delta)
	{
		ReplayEditor3DWindow.Update(delta);
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		if (ImGui.Begin("Replay Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
		{
			ImGui.PopStyleVar();

			ReplayEditorMenu.Render();

			ReplayFileInfo.Render();

			if (ImGui.BeginTabBar("Replay Editor Tabs"))
			{
				if (ImGui.BeginTabItem("Events"))
				{
					ReplayEventsChild.Render(FileStates.Replay.Object.EventsData, FileStates.Replay.Object.Header.StartTime);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities"))
				{
					ReplayEntitiesChild.Render(FileStates.Replay.Object.EventsData, FileStates.Replay.Object.Header.StartTime);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Inputs"))
				{
					ReplayInputsChild.Render(FileStates.Replay.Object.EventsData, FileStates.Replay.Object.Header.StartTime);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Debug"))
				{
					if (ImGui.BeginChild("ReplayDebugChild", new(0, 0)))
					{
						ImGui.Text("Event counts per tick:");

						for (int i = 0; i < FileStates.Replay.Object.EventsData.EventOffsetsPerTick.Count; i++)
						{
							int offset = FileStates.Replay.Object.EventsData.EventOffsetsPerTick[i];
							ImGui.Text(Inline.Span($"{i} ({TimeUtils.TickToTime(i, 0):0.0000}): {offset}"));
						}

						ImGui.EndTabItem();
					}

					ImGui.EndChild(); // ReplayDebugChild
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
