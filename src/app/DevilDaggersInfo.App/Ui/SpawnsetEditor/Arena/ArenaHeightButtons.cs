using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;

public static class ArenaHeightButtons
{
	private const int _arenaButtonSize = 24;
	private const int _arenaButtonSizeLarge = 48;

	public static void Render()
	{
		ImGui.BeginChild("ArenaHeightButtons", new(388, 112));

		Span<float> heights = stackalloc float[] { -1000, -1.1f, -1.01f, -1, -0.8f, -0.6f, -0.4f, -0.2f };
		for (int i = 0; i < heights.Length; i++)
		{
			float height = heights[i];
			int offsetX = i % 2 * _arenaButtonSizeLarge;
			int offsetY = i / 2 * _arenaButtonSize;
			AddHeightButton(height, offsetX, offsetY, _arenaButtonSizeLarge);
		}

		for (int i = 0; i < 48; i++)
		{
			int offsetX = i % 12 * _arenaButtonSize;
			int offsetY = i / 12 * _arenaButtonSize;
			AddHeightButton(i, offsetX + _arenaButtonSizeLarge * 2, offsetY);
		}

		ImGui.EndChild();

		static void AddHeightButton(float height, int offsetX, int offsetY, int width = _arenaButtonSize)
		{
			Color heightColor = TileUtils.GetColorFromHeight(height);

			const int borderSize = 2;
			ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
			ImGui.PushStyleColor(ImGuiCol.Text, height < 2 ? Color.White : Color.Black);
			ImGui.PushStyleColor(ImGuiCol.Button, heightColor);
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.Lerp(heightColor, Color.White, 0.75f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Lerp(heightColor, Color.White, 0.5f));
			ImGui.PushStyleColor(ImGuiCol.Border, Math.Abs(ArenaChild.SelectedHeight - height) < 0.001f ? Color.Invert(heightColor) : Color.Lerp(heightColor, Color.Black, 0.2f));

			ImGui.SetCursorPos(new(offsetX + borderSize * 2, offsetY + borderSize));
			if (ImGui.Button(UnsafeSpan.Get(height), new(width - 1, _arenaButtonSize - 1)))
				ArenaChild.SelectedHeight = height;

			ImGui.PopStyleColor(5);
			ImGui.PopStyleVar();
		}
	}
}
