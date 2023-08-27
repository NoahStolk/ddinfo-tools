using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.ReplayEditor.Events;
using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.App.Ui.ReplayEditor.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events.Enums;
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

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();

			// Pointer area
			const int pointerBorder = 4;
			const int pointerSize = 4;
			const float pointerScale = 0.25f;
			const int mousePointerAreaSize = 64;
			drawList.AddRect(origin, origin + new Vector2(mousePointerAreaSize), 0xFFFFFFFF);
			drawList.AddRectFilled(origin + new Vector2(pointerBorder), origin + new Vector2(mousePointerAreaSize - pointerBorder), 0xFF000000);

			// Pointer
			const int center = mousePointerAreaSize / 2;
			const float max = center - pointerBorder - pointerScale / 2;
			Vector2 mouseCoordinates = new(snapshot.MouseX, snapshot.MouseY);
			Vector2 pointerCenter = origin + new Vector2(center) + Clamp(mouseCoordinates * pointerScale, -max, max);
			drawList.AddRect(pointerCenter - new Vector2(pointerSize / 2f), pointerCenter + new Vector2(pointerSize / 2f), 0xFFFFFFFF);
			drawList.AddText(origin + new Vector2(0, mousePointerAreaSize), 0xFFFFFFFF, UnsafeSpan.Get(mouseCoordinates, "+000;-000;+000"));

			// Inputs
			const int inputSize = 32;
			Vector2 inputRect = new(inputSize);
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize, inputSize), inputRect, snapshot.Left, "A");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize * 2, inputSize), inputRect, snapshot.Right, "D");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize, 0), inputRect, snapshot.Forward, "W");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize, inputSize), inputRect, snapshot.Backward, "S");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize * 3, inputSize), new(64, 32), snapshot.Jump is JumpType.StartedPress or JumpType.Hold, "Space");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize * 3, 0), inputRect, snapshot.Shoot == ShootType.Hold, "LMB");
			RenderInput(drawList, origin + new Vector2(mousePointerAreaSize + inputSize * 4, 0), inputRect, snapshot.ShootHoming == ShootType.Hold, "RMB");

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

		static Vector2 Clamp(Vector2 vector, float min, float max)
		{
			return new(Math.Clamp(vector.X, min, max), Math.Clamp(vector.Y, min, max));
		}
	}

	private static void RenderInput(ImDrawListPtr drawList, Vector2 position, Vector2 size, bool used, ReadOnlySpan<char> input)
	{
		uint color = ImGui.GetColorU32(used ? Color.Red : Color.White);
		drawList.AddRect(position, position + size, color);

		Vector2 textSize = ImGui.CalcTextSize(input);
		drawList.AddText(position + (size - textSize) / 2, color, input);
	}
}
