using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

internal sealed class ArenaHeightButtons(ArenaStateManager arenaStateManager)
{
	private const int _arenaButtonSize = 24;
	private const int _arenaButtonSizeLarge = 48;

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
			Vector4 heightColor = TileUtils.GetColorFromHeight(height);

			const int borderSize = 2;
			ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderSize);
			ImGui.PushStyleColor(ImGuiCol.Text, height < 2 ? Vector4.One : new Vector4(0, 0, 0, 1));
			ImGui.PushStyleColor(ImGuiCol.Button, heightColor);
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Lerp(heightColor, Vector4.One, 0.75f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Lerp(heightColor, Vector4.One, 0.5f));
			ImGui.PushStyleColor(ImGuiCol.Border, Math.Abs(arenaStateManager.SelectedHeight - height) < 0.001f ? Vector4.InvertColor(heightColor) : Vector4.Lerp(heightColor, new Vector4(0, 0, 0, 1), 0.2f));

			ImGui.SetCursorPos(new Vector2(offsetX + borderSize * 2, offsetY + borderSize));
			if (ImGui.Button(Inline.Utf8(height), new Vector2(width - 1, _arenaButtonSize - 1)))
				arenaStateManager.SelectedHeight = height;

			ImGui.PopStyleColor(5);
			ImGui.PopStyleVar();
		}
	}
}
