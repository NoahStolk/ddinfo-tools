using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayEditorWindow
{
	private static float _gameMemoryTimer = 5;

	public static void Reset()
	{
		ReplayEditor3DWindow.Reset();
		ReplayEventsViewerChild.Reset();
		ReplayEntitiesChild.Reset();
		ReplayTimelineChild.Reset();
	}

	public static void Update(float delta)
	{
		ReplayEditor3DWindow.Update(delta);

		_gameMemoryTimer += delta;
		if (_gameMemoryTimer < 5f)
			return;

		_gameMemoryTimer = 0;
		GameMemoryServiceWrapper.Scan();
	}

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin("Replay Editor", ImGuiWindowFlags.NoCollapse))
		{
			ReplayFileInfo.Render();

			if (ImGui.BeginTabBar("Replay Editor Tabs"))
			{
				if (ImGui.BeginTabItem("Events editor"))
				{
					ReplayTimelineChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Events viewer"))
				{
					ReplayEventsViewerChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities viewer"))
				{
					ReplayEntitiesChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Inputs viewer"))
				{
					ReplayInputsChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Shotgun speed"))
				{
					ReplayShotgunSpeedChild.Render(FileStates.Replay.Object.EventsData);
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End();
	}
}
