using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayEditorWindow
{
	private static float _gameMemoryTimer = 5;

	public static void Reset()
	{
		ReplayEditor3DWindow.Reset();
		ReplayEventsChild.Reset();
		ReplayEntitiesChild.Reset();
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

	public static unsafe void Render()
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

#if DEBUG
				if (ImGui.BeginTabItem("Debug"))
				{
					if (ImGui.BeginChild("ReplayDebugChild", new(0, 0)))
					{
						ImGui.Text("Event counts per tick:");

						ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
						clipper.Begin(FileStates.Replay.Object.EventsData.EventOffsetsPerTick.Count);
						while (clipper.Step())
						{
							for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
							{
								int offset = FileStates.Replay.Object.EventsData.EventOffsetsPerTick[i];
								ImGui.Text(Inline.Span($"{i} ({TimeUtils.TickToTime(i, 0):0.0000}): {offset}"));
							}
						}

						clipper.End();
					}

					ImGui.EndChild(); // ReplayDebugChild

					ImGui.EndTabItem();
				}
#endif

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
