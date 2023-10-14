using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
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
