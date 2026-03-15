using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class ReplayEditorWindow(
	ReplayEventsViewerChild replayEventsViewerChild,
	ReplayEntitiesChild replayEntitiesChild,
	ReplayInputsChild replayInputsChild,
	ReplayTimelineChild replayTimelineChild,
	ReplayEditor3DWindow replayEditor3DWindow,
	FileStates fileStates,
	GameMemoryServiceWrapper gameMemoryServiceWrapper)
{
	private float _gameMemoryTimer = 5;

	public void Reset()
	{
		replayEditor3DWindow.Reset();
		replayEventsViewerChild.Reset();
		replayEntitiesChild.Reset();
		replayTimelineChild.Reset();
	}

	public void Update(float delta)
	{
		replayEditor3DWindow.Update(delta);

		_gameMemoryTimer += delta;
		if (_gameMemoryTimer < 5f)
			return;

		_gameMemoryTimer = 0;
		gameMemoryServiceWrapper.Scan();
	}

	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin("Replay Editor", ImGuiWindowFlags.NoCollapse))
		{
			ReplayFileInfo.Render(fileStates.Replay.Object);

			if (ImGui.BeginTabBar("Replay Editor Tabs"))
			{
				if (ImGui.BeginTabItem("Events editor"))
				{
					replayTimelineChild.Render(fileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Events viewer"))
				{
					replayEventsViewerChild.Render(fileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities viewer"))
				{
					replayEntitiesChild.Render(fileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Inputs viewer"))
				{
					replayInputsChild.Render(fileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End();
	}
}
