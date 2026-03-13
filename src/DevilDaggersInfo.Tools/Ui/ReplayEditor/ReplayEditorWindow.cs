using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class ReplayEditorWindow(
	ReplayEventsViewerChild replayEventsViewerChild,
	ReplayEntitiesChild replayEntitiesChild,
	ReplayInputsChild replayInputsChild,
	ReplayEditor3DWindow replayEditor3DWindow)
{
	private float _gameMemoryTimer = 5;

	public void Reset()
	{
		replayEditor3DWindow.Reset();
		replayEventsViewerChild.Reset();
		replayEntitiesChild.Reset();
		ReplayTimelineChild.Reset();
	}

	public void Update(float delta)
	{
		replayEditor3DWindow.Update(delta);

		_gameMemoryTimer += delta;
		if (_gameMemoryTimer < 5f)
			return;

		_gameMemoryTimer = 0;
		GameMemoryServiceWrapper.Scan();
	}

	public void Render()
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
					replayEventsViewerChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities viewer"))
				{
					replayEntitiesChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Inputs viewer"))
				{
					replayInputsChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End();
	}
}
