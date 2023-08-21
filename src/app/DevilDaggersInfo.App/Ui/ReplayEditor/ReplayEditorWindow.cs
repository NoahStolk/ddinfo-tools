using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEditorWindow
{
	// TODO: Move to 3D window.
	private static float _time;

	// TODO: Move to 3D window.
	public static void Reset()
	{
		_time = 0;
	}

	// TODO: Move to 3D window.
	public static void Update(float delta)
	{
		if (_time < ReplayState.Replay.Header.Time)
			_time += delta;

		ReplayEditor3DWindow.ArenaScene.CurrentTick = (int)MathF.Round(_time * 60);
	}

	public static void Render()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		if (ImGui.Begin("Replay Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse))
		{
			ImGui.PopStyleVar();

			ReplayEditorMenu.Render();

			ImGui.SliderFloat("Time", ref _time, 0, ReplayState.Replay.Header.Time, "%.4f", ImGuiSliderFlags.NoInput);

			PlayerInputSnapshot snapshot = default;
			if (ReplayEditor3DWindow.ArenaScene.CurrentTick < ReplayEditor3DWindow.ArenaScene.ReplaySimulation?.InputSnapshots.Count)
				snapshot = ReplayEditor3DWindow.ArenaScene.ReplaySimulation.InputSnapshots[ReplayEditor3DWindow.ArenaScene.CurrentTick];

			const int border = 4;
			const int pointerSize = 4;
			const float pointerScale = 0.25f;
			const int size = 64;
			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 origin = ImGui.GetCursorScreenPos();
			drawList.AddRect(origin, origin + new Vector2(size), ImGui.GetColorU32(Color.Gray(0.6f)));
			drawList.AddRectFilled(origin + new Vector2(border), origin + new Vector2(size - border), ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

			const int center = size / 2;
			const float max = center - border - pointerScale / 2;
			Vector2 pointerCenter = origin + new Vector2(center) + Clamp(new Vector2(snapshot.MouseX, snapshot.MouseY) * pointerScale, -max, max);
			drawList.AddRect(pointerCenter - new Vector2(pointerSize / 2f), pointerCenter + new Vector2(pointerSize / 2f), ImGui.GetColorU32(Color.White));

			RenderInput(snapshot.Left, "A");
			RenderInput(snapshot.Right, "D");
			RenderInput(snapshot.Forward, "W");
			RenderInput(snapshot.Backward, "S");
			RenderInput(snapshot.Jump is JumpType.StartedPress or JumpType.Hold, "Space");
			RenderInput(snapshot.Shoot == ShootType.Hold, "LMB");
			RenderInput(snapshot.ShootHoming == ShootType.Hold, "RMB");
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

	private static void RenderInput(bool used, string input)
	{
		// TODO: Position the inputs.
		Color color = used ? Color.Red : Color.White;
		ImGui.TextColored(color, input);
	}
}
