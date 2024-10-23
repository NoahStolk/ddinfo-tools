using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

public sealed class ArenaHeightButtons
{
	private const int _arenaButtonSize = 24;
	private const int _arenaButtonSizeLarge = 48;

	private readonly ArenaWindow _arenaWindow;

	public ArenaHeightButtons(ArenaWindow arenaWindow)
	{
		_arenaWindow = arenaWindow;
	}

	public void Render()
	{
		if (ImGui.BeginChild("ArenaHeightButtons", new Vector2(388, 112)))
		{
			Span<float> heights = [-1000, -1.1f, -1.01f, -1, -0.8f, -0.6f, -0.4f, -0.2f];
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
		}

		ImGui.EndChild();

		void AddHeightButton(float height, int offsetX, int offsetY, int width = _arenaButtonSize)
		{
			Color heightColor = TileUtils.GetColorFromHeight(height);

			const int borderSize = 2;
			ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
			ImGui.PushStyleColor(ImGuiCol.Text, height < 2 ? Color.White : Color.Black);
			ImGui.PushStyleColor(ImGuiCol.Button, heightColor);
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, Color.Lerp(heightColor, Color.White, 0.75f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Color.Lerp(heightColor, Color.White, 0.5f));
			ImGui.PushStyleColor(ImGuiCol.Border, Math.Abs(_arenaWindow.SelectedHeight - height) < 0.001f ? Color.Invert(heightColor) : Color.Lerp(heightColor, Color.Black, 0.2f));

			ImGui.SetCursorPos(new Vector2(offsetX + borderSize * 2, offsetY + borderSize));
			if (ImGui.Button(Inline.Span(height), new Vector2(width - 1, _arenaButtonSize - 1)))
				_arenaWindow.SelectedHeight = height;

			ImGui.PopStyleColor(5);
			ImGui.PopStyleVar();
		}
	}
}
