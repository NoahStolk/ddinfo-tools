using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public sealed class ReplayEditorWindow
{
	private readonly ReplayEventsViewerChild _replayEventsViewerChild;
	private readonly ReplayEntitiesChild _replayEntitiesChild;
	private readonly ReplayInputsChild _replayInputsChild;
	private readonly ReplayEditor3DWindow _replayEditor3DWindow;

	private float _gameMemoryTimer = 5;

	public ReplayEditorWindow(ReplayEventsViewerChild replayEventsViewerChild, ReplayEntitiesChild replayEntitiesChild, ReplayInputsChild replayInputsChild, ReplayEditor3DWindow replayEditor3DWindow)
	{
		_replayEventsViewerChild = replayEventsViewerChild;
		_replayEntitiesChild = replayEntitiesChild;
		_replayInputsChild = replayInputsChild;
		_replayEditor3DWindow = replayEditor3DWindow;
	}

	public void Reset()
	{
		_replayEditor3DWindow.Reset();
		_replayEventsViewerChild.Reset();
		_replayEntitiesChild.Reset();
		ReplayTimelineChild.Reset();
	}

	public void Update(float delta)
	{
		_replayEditor3DWindow.Update(delta);

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
					_replayEventsViewerChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entities viewer"))
				{
					_replayEntitiesChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Inputs viewer"))
				{
					_replayInputsChild.Render(FileStates.Replay.Object);
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.End();
	}
}
