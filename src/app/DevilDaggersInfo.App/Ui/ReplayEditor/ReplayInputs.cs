using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayInputs
{
	public static void Render(Vector2 origin, PlayerInputSnapshot snapshot)
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

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
	}

	private static void RenderInput(ImDrawListPtr drawList, Vector2 position, Vector2 size, bool used, ReadOnlySpan<char> input)
	{
		uint color = ImGui.GetColorU32(used ? Color.Red : Color.White);
		drawList.AddRect(position, position + size, color);

		Vector2 textSize = ImGui.CalcTextSize(input);
		drawList.AddText(position + (size - textSize) / 2, color, input);
	}

	private static Vector2 Clamp(Vector2 vector, float min, float max)
	{
		return new(Math.Clamp(vector.X, min, max), Math.Clamp(vector.Y, min, max));
	}
}
